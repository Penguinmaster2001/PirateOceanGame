
using Godot;

using System.Linq;
using System.Collections.Generic;


namespace HexModule.Map
{
	public class HexMap : HexContainer
	{
		public List<WfcHex> CollapsedHexes { get; protected set; }

		public List<WfcHex> UncollapsedHexes { get; protected set; }

		private HexShortList shortlist = new();

		private readonly RandomNumberGenerator rng = new();



		public HexMap(MapShape shape, int size) : base(shape, size)
		{
			CollapsedHexes = new();
			UncollapsedHexes = new();

			foreach (WfcHex hex in hexes.Values)
				UncollapsedHexes.Add(hex);
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



		public void PopulateHexType(int num, string hexTypeName)
		{
			HashSet<HexType> hexTypesWithName = HexTypesCollection.GetTypesWithName(hexTypeName);
		
			for (int i = 0; i < num; i++)
			{
				HexType randomHexType = hexTypesWithName.ElementAt((System.Index) (rng.Randi() % hexTypesWithName.Count));
		
				CollapseHex(UncollapsedHexes[rng.RandiRange(0, UncollapsedHexes.Count - 1)], randomHexType);
			}
		}



		public bool TryCollapseNextHex(out WfcHex collapsedHex)
		{
			if (!TryGetMostConstrainedHex(out collapsedHex)) return false;

			CollapseHex(collapsedHex, collapsedHex.GetRandomAllowedType());

			return true;
		}



		// Returns true and assigns mostConstrained hex if not all hexes are collapsed
		// Otherwise return false and mostConstrained is null
		private bool TryGetMostConstrainedHex(out WfcHex mostConstrainedHex)
		{
			mostConstrainedHex = null;

            if (AllHexesCollapsed()) return false;

            if (!shortlist.TryGetMostConstrainedHex(out mostConstrainedHex))
				mostConstrainedHex = UncollapsedHexes[rng.RandiRange(0, UncollapsedHexes.Count - 1)];

			return true;
		}



		/*
		* Remove the hex from the shortlist and uncollapsed list
		* Tell the hex to collapse
		* Constrain the neighbors
		* Constrain the neighbors' neighbors
		*/
		public void CollapseHex(WfcHex hex, HexType hexType)
		{
			if (hex is null || hex.Collapsed) return;
			
			CollapseHexAndUpdateLists(hex, hexType);

			if (hexType.Weight == 0) return;

			Hex[] neighbors = GetHexNeighbors(hex);

			// Constrain the neighbor on each edge, and their neighbors
			for (int edgeIndex = 0; edgeIndex < Hex.NumEdges; edgeIndex++)
			{
				if (!TryGetNeighbor(neighbors, edgeIndex, out WfcHex neighbor)) continue;

				HashSet<EdgeType> validEdgeTypes = hex.GetValidEdgeTypes(edgeIndex);
				
				ConstrainNeighborAndBeyond(neighbor, validEdgeTypes, edgeIndex, 2);
			}
		}



		private void CollapseHexAndUpdateLists(WfcHex hex, HexType hexType)
		{
			CollapsedHexes.Add(hex);
			UncollapsedHexes.Remove(hex);
			shortlist.Remove(hex);

			hex.Collapse(hexType);
		}



		private void ConstrainNeighborAndBeyond(WfcHex neighbor, HashSet<EdgeType> validEdgeTypes, int edgeIndex, int depth)
		{
			// Constrain edge adjacent to previous hex





			// Average: 7.947240259740259
			// The neighbor's edge index is opposite the collapsed hex's
			const int EdgeOffset = 3;

			// Constrain the edge adjacent to the collapsed hex's edge
			ConstrainAndUpdateShortlist(neighbor, validEdgeTypes, edgeIndex + EdgeOffset);

			if (depth <= 0) return;

			Hex[] neighborNeighbors = GetHexNeighbors(neighbor);

			// Constrain the neighbor's neighbors, excluding the original collapsed hex
			// This loops through edge indices 4, 5, 0, 1, 2 (mod 6)
			for (int neighborEdgeIndex = 4 + edgeIndex; neighborEdgeIndex <= 8 + edgeIndex; neighborEdgeIndex++)
			{
				if (!TryGetNeighbor(neighborNeighbors, neighborEdgeIndex, out WfcHex neighborNeighbor)) continue;

				HashSet<EdgeType> neighborValidEdgeTypes = neighbor.GetValidEdgeTypes(neighborEdgeIndex);
				
				ConstrainNeighborAndBeyond(neighborNeighbor, neighborValidEdgeTypes, neighborEdgeIndex, depth - 1);
			}
			
			// Average: 7.8149300155520995
			// const int EdgeOffset = 3;
			// var stack = new Stack<(WfcHex, HashSet<EdgeType>, int, int)>();
			// stack.Push((neighbor, validEdgeTypes, edgeIndex, depth));

			// // Average: 7.733743747595229
			// Dictionary<WfcHex, Hex[]> neighborsCache = new();

			// while (stack.Count > 0)
			// {
			// 	(WfcHex currentNeighbor, HashSet<EdgeType> currentValidEdgeTypes, int currentEdgeIndex, int currentDepth) = stack.Pop();

			// 	ConstrainAndUpdateShortlist(currentNeighbor, currentValidEdgeTypes, currentEdgeIndex + EdgeOffset);

			// 	if (currentDepth <= 0) continue;

			// 	// Hex[] neighborNeighbors = GetHexNeighbors(currentNeighbor);
			// 	if (!neighborsCache.TryGetValue(currentNeighbor, out Hex[] neighborNeighbors))
			// 	{
			// 		neighborNeighbors = GetHexNeighbors(currentNeighbor);
			// 		neighborsCache.Add(currentNeighbor, neighborNeighbors);
			// 	}
				

			// 	for (int neighborEdgeIndex = 4 + currentEdgeIndex; neighborEdgeIndex <= 8 + currentEdgeIndex; neighborEdgeIndex++)
			// 	{
			// 		if (!TryGetNeighbor(neighborNeighbors, neighborEdgeIndex, out WfcHex neighborNeighbor)) continue;

			// 		HashSet<EdgeType> neighborNeighborValidEdgeTypes = currentNeighbor.GetValidEdgeTypes(neighborEdgeIndex);

			// 		stack.Push((neighborNeighbor, neighborNeighborValidEdgeTypes, neighborEdgeIndex, currentDepth - 1));
			// 	}
			// }
		}



		// Get the neighbor adjacent to the edge, return null if it isn't nice
		private static bool TryGetNeighbor(Hex[] neighbors, int edgeIndex, out WfcHex neighbor)
		{
			edgeIndex %= Hex.NumEdges;

			neighbor = null;

			if (neighbors[edgeIndex] is null or not WfcHex) return false;

			WfcHex validNeighbor = (WfcHex) neighbors[edgeIndex];

			if (validNeighbor.Collapsed) return false;

			neighbor = validNeighbor;

			return true;
		}



		private void ConstrainAndUpdateShortlist(WfcHex hex, HashSet<EdgeType> validEdgeTypes, int edgeIndex)
		{
			shortlist.Remove(hex);

			hex.ConstrainEdge(edgeIndex, validEdgeTypes);

			shortlist.Insert(hex);
		}



		public bool AllHexesCollapsed()
		{
			return UncollapsedHexes.Count == 0;
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
