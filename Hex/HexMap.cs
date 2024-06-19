using Godot;
using System.Collections.Generic;


namespace HexModule
{
	public class HexMap : HexContainer
	{
		public List<WfcHex> CollapsedHexes { get; protected set; }

		public List<WfcHex> UncollapsedHexes { get; protected set; }

		private HexShortList shortlist = new();



		public HexMap(MapShape shape, int size) : base(shape, size)
		{
			CollapsedHexes = new();
			UncollapsedHexes = new();

			foreach (WfcHex hex in hexes.Values)
			{
				UncollapsedHexes.Add(hex);
			}
		}



		// Add a hex to the map at coords
		public new void InsertHex(int q, int r)
		{
			base.InsertHex(q, r);

			UncollapsedHexes.Add(GetHex(q, r));
		}



		public Hex HexAtWorldCoordinates(float x, float y)
		{
			Hex nearest_hex = Hex.WorldCoordsToHex(x, y);

			return GetHex(nearest_hex.Q, nearest_hex.R);
		}



		public void PopulateHexType(int num, HexType hexType)
		{
			RandomNumberGenerator rng = new();

			for (int i = 0; i < num; i++)
			{
				CollapseHex(UncollapsedHexes[rng.RandiRange(0, UncollapsedHexes.Count - 1)], hexType);
			}
		}



		public Hex CollapseNextHex()
		{
			WfcHex mostConstrainedHex = GetMostConstrainedHex();

			if (mostConstrainedHex != null)
			{
				CollapseHex(mostConstrainedHex, mostConstrainedHex.GetRandomAllowedType());

				return mostConstrainedHex;
			}
			
			return null;
		}



		// Return the most constrained tile, or a random uncollapsed one if the shortlist is empty
		private WfcHex GetMostConstrainedHex()
		{
			RandomNumberGenerator rng = new();

			if (UncollapsedHexes.Count == 0) return null;

			if (shortlist.IsEmpty())
				return UncollapsedHexes[rng.RandiRange(0, UncollapsedHexes.Count - 1)];
			
			return shortlist.GetRandomMostConstrained();
		}



		private void CollapseHexAtCoords(int q, int r, HexType hexType)
		{
			Hex hex = GetHex(q, r);

			if (hex is null or not WfcHex) return;

			CollapseHex(hex as WfcHex, hexType);
		}



		/*
		* Remove the hex from the shortlist and uncollapsed list
		* Tell the hex to collapse
		* Constrain the neighbors
		* This is so messy it's not even funny
		*/
		public void CollapseHex(WfcHex hex, HexType hexType)
		{
			if (hex.Collapsed) return;
			
			CollapsedHexes.Add(hex);
			UncollapsedHexes.Remove(hex);
			shortlist.Remove(hex);

			hex.Collapse(hexType);

			// Constrain each neighbor
			Hex[] neighbors = GetHexNeighbors(hex);

			for (int i = 0; i < 6; i++)
			{
				if (neighbors[i] is null || neighbors[i] is not WfcHex neighbor || neighbor.Collapsed) continue;

				// Keep track of this for update_or_insert()
				int previous_num = neighbor.Constraint;

				// Constrain the adjacent edge on the neighbor, which is offset by 3
				neighbor.ConstrainEdge(i + 3, hex.GetValidEdgeTypes(i));

				shortlist.UpdateOrInsert(neighbor, previous_num);

				// Hex past neighbor
				Hex[] neighbor_neighbors = GetHexNeighbors(neighbor);

				if (neighbor_neighbors[i] is null or not WfcHex) continue;

				WfcHex second_neighbor = neighbor_neighbors[i] as WfcHex;

				if (second_neighbor.Collapsed) continue;

				previous_num = second_neighbor.Constraint;

				second_neighbor.ConstrainEdge(i + 3, neighbor.GetValidEdgeTypes(i));

				shortlist.UpdateOrInsert(second_neighbor, previous_num);
				
				for (int j = 4; j <= 8; j++)
					constrain_neighbor(j % 6);

				void constrain_neighbor(int edgeIndex)
				{

					if (neighbor_neighbors[edgeIndex] is null or not WfcHex) return;

					WfcHex neighbor_to_constrain = neighbor_neighbors[edgeIndex] as WfcHex;

					if (neighbor_to_constrain.Collapsed) return;
					

					int previous_num = neighbor_to_constrain.Constraint;

					neighbor_to_constrain.ConstrainEdge(edgeIndex + 3, neighbor.GetValidEdgeTypes(edgeIndex));

					shortlist.UpdateOrInsert(neighbor_to_constrain, previous_num);
				}
			}
		}


		/*
		* Return the path between two hexes
		* Tries to make as many straight lines as possible
		* Uses a modified version of the A* alg
		*/
		public List<Hex> FindPathBetween(Hex start, Hex end)
		{
			start ??= GetHex(0, 0);

			if (end is null) return new();

			if (!start.TerrainType.IsTraversable || !end.TerrainType.IsTraversable) return new();

			// Search from end to start
			PriorityQueue<Hex, float> frontier = new();
			frontier.Enqueue(end, 0);
			List<Hex> reached = new() { end };
			Dictionary<Hex, Hex> previousHex = new() {{ end, null }};
			Dictionary<Hex, float> hexCostMap = new() {{ end, 0 }};
			Dictionary<Hex, bool> traversableBounds = new() {{ end, true }};

			int searched = 0;



			while (frontier.Count > 0)
			{
				searched++;
				if (searched > 10000)
					return new();

				Hex current = frontier.Dequeue();

				if (current == start)
					break;
				
				foreach (Hex next in GetHexNeighbors(current))
				{
					if (next is null)
						continue;
					
					if (!next.TerrainType.IsTraversable)
					{
						traversableBounds[current] = true;
						continue;
					}

					float updatedCost = hexCostMap[current];
					if (!reached.Contains(next))
					{
						reached.Add(next);
						previousHex.Add(next, current);
						traversableBounds.Add(next, false);
						hexCostMap.Add(next, updatedCost);
						frontier.Enqueue(next, updatedCost + Hex.GetWorldDistance(next, start));
					}
					else if (updatedCost < hexCostMap[next])
					{
						previousHex[next] = current;
						hexCostMap[next] = updatedCost;
						traversableBounds[next] = false;
						frontier.Enqueue(next, updatedCost + Hex.GetWorldDistance(next, start));
					}
				}
			}

			// Build list from start to end
			List<Hex> path = new();

			{
				Hex line_start = start;
				Hex current = start;

				while (current != end)
				{
					current = previousHex[current];

					if (traversableBounds[current])
					{
						path.AddRange(FindLineBetween(line_start, current));
						line_start = current;
					}
				}

				path.Add(end);

				// Hex current = start;

				// while (current != end)
				// {
				// 	current = came_from[current];

				// 	path.Add(current);
				// }

				// path.Add(end);
			}

			return path;
		}



		/*
		* Find the hexes on the line between two hexes, and return a list of them in order
		* Includes end, does not include start
		*/
		private List<Hex> FindLineBetween(Hex start, Hex end)
		{
			start ??= end;

			if (end == null) return new();

			// If start == end we get a division by zero
			if (start == end) return new() {start};

			int dist = Hex.GetHexDistance(start, end);
			List<Hex> hexesOnLine = new();

			for (int i = 0; i < dist + 1; i++)
			{
				Hex closestHex = Hex.Lerp(start, end, (float) i / (float) dist);
				hexesOnLine.Add(GetHex(closestHex.Q, closestHex.R));
			}

			return hexesOnLine;
		}



		public bool AllHexesCollapsed()
		{
			return (UncollapsedHexes.Count == 0);
		}



		public new void Clear()
		{
			size = 0;
			hexes = new();
			CollapsedHexes = new();

			UncollapsedHexes = new();

			shortlist = new();
		}
	}
}
