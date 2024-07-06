
using System.Collections.Generic;

namespace HexModule.Map
{
    public class Navigator
    {
        private readonly HexMap map;
        private readonly HashSet<NavigableTag> navigableTags;
        private const int MaxSearch = 10000;



        public Navigator(HexMap map, HashSet<NavigableTag> navigableTags)
        {
            this.navigableTags = navigableTags;
            this.map = map;
        }



        public bool TryNavigateStraight(Hex start, Hex end, Queue<Hex> waypointsQueue)
        {
            if (!DiscoverGraphBetween(start, end,
                out Dictionary<Hex, Hex> previousHex,
                out Dictionary<Hex, bool> traversableBounds))
                    return false;



			// Build list from start to end
            Hex line_start = start;
            Hex current = start;

            while (current != end)
            {
                current = previousHex[current];

                if (traversableBounds[current])
                {
                    FindLineBetween(line_start, current, waypointsQueue);

                    line_start = current;
                }
            }

            waypointsQueue.Enqueue(end);

            // Hex current = start;

            // while (current != end)
            // {
            // 	current = came_from[current];

            // 	path.Add(current);
            // }

            // path.Add(end);

			return true;
        }



        /// <summary>
        /// Discovers a graph from the end hex to the start hex in the form of a dictionary
        /// that maps a hex to the one before it
        /// </summary>
        /// <param name="start">The start of the search</param>
        /// <param name="end">The end of the search</param>
        /// <param name="graph">The dictionary to be filled with the graph</param>
        /// <param name="traversableBounds">The dictionary that holds whether a
        /// hex is adjacent to a non-navigable hex</param>
        /// <returns>
        /// true: if a navigable path could be found after visiting less than MaxSearch hexes
        /// false: if no path from start to end could be found or more than MaxSearch hexes have been visited
        /// </returns>
        private bool DiscoverGraphBetween(Hex start, Hex end,
            out Dictionary<Hex, Hex> graph, out Dictionary<Hex, bool> traversableBounds)
        {
			graph = new() {{ end, null }};
			traversableBounds = new() {{ end, true }};
            
            start ??= map.GetHex(0, 0);

			if (end is null) return false;

			if (!IsNavigable(start) || !IsNavigable(end)) return false;

			// Search from end to start so the graph points the right way
			PriorityQueue<Hex, float> frontier = new();
			frontier.Enqueue(end, 0);
			HashSet<Hex> reached = new() { end };
			Dictionary<Hex, float> hexCostMap = new() {{ end, 0 }};

			int searched = 0;

			while (frontier.Count > 0)
			{
				searched++;
				if (searched > MaxSearch) return false;

				Hex current = frontier.Dequeue();

				if (current == start) break;
				

				foreach (Hex next in map.GetHexNeighbors(current))
				{
					if (next is null) continue;
					
					if (!IsNavigable(next))
					{
						traversableBounds[current] = true;
						continue;
					}

					float updatedCost = hexCostMap[current];
					if (!reached.Contains(next))
					{
						reached.Add(next);
						graph.Add(next, current);
						traversableBounds.Add(next, false);
						hexCostMap.Add(next, updatedCost);
						frontier.Enqueue(next, updatedCost + Hex.GetWorldDistance(next, start));
					}
					else if (updatedCost < hexCostMap[next])
					{
						graph[next] = current;
						hexCostMap[next] = updatedCost;
						traversableBounds[next] = false;
						frontier.Enqueue(next, updatedCost + Hex.GetWorldDistance(next, start));
					}
				}
			}

            return true;
        }


        /// <summary>
        /// Find the hexes on the line between two hexes, and return a list of them in order
        /// </summary>
        /// <param name="start">The hex the line starts at</param>
        /// <param name="end">The hex the line ends at</param>
        /// <param name="waypointsQueue">The queue the line will be added to</param>
        private void FindLineBetween(Hex start, Hex end, Queue<Hex> waypointsQueue)
        {
            start ??= end;

            if (end == null) return;

            waypointsQueue.Enqueue(start);

            // If start == end we get a division by zero
            if (start == end) return;

            int dist = Hex.GetHexDistance(start, end);

            for (int i = 0; i < dist + 1; i++)
            {
                Hex closestHex = Hex.Lerp(start, end, (float)i / (float)dist);
                waypointsQueue.Enqueue(map.GetHex(closestHex.Q, closestHex.R));
            }
        }



        private bool IsNavigable(Hex hex)
        {
            return navigableTags.Contains(hex.TerrainType.NavigableTag);
        }
    }
}
