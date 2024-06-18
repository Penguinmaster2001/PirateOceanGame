using Godot;
using System.Collections.Generic;

public class HexMap : HexContainer
{
	private List<WfcHex> collapsed = new();
	public List<WfcHex> get_collapsed_hexes() => collapsed;

	private List<WfcHex> not_collapsed = new();
	public List<WfcHex> get_uncollapsed_hexes() => not_collapsed;

	private HexShortList shortlist = new();



	public HexMap(MapShape shape, int size) : base(shape, size)
	{
		foreach (WfcHex hex in hexes.Values)
		{
			not_collapsed.Add(hex);
		}
	}



	// Add a hex to the map at coords
	public new void add_hex(int q, int r)
	{
		base.add_hex(q, r);

		not_collapsed.Add(get_hex(q, r));
	}



	public Hex get_hex_at_world_coords(float x, float y)
	{
		Hex nearest_hex = Hex.world_coords_to_hex(x, y);

		return get_hex(nearest_hex.get_q(), nearest_hex.get_r());
	}



	public void seed_type(int num, string type_name)
	{
		RandomNumberGenerator rng = new();

		int type = HexTypes.get_type_from_name(type_name);

		for (int i = 0; i < num; i++)
		{
			collapse_hex(not_collapsed[rng.RandiRange(0, not_collapsed.Count - 1)], type);
		}
	}



	public Hex collapse_next_hex()
	{
		WfcHex next_to_collapse = get_most_constrained();

		if (next_to_collapse != null)
		{
			collapse_hex(next_to_collapse, next_to_collapse.get_random_allowed_type());

			return next_to_collapse;
		}
		
		return null;
	}



	// Return the most constrained tile, or a random uncollapsed one if the shortlist is empty
	private WfcHex get_most_constrained()
	{
		RandomNumberGenerator rng = new();

		if (not_collapsed.Count == 0)
			return null;

		if (shortlist.is_empty())
			return not_collapsed[rng.RandiRange(0, not_collapsed.Count - 1)];
		
		return shortlist.get_most_constrained_random();
	}


	private void collapse_coords(int q, int r, string type_name)
	{
		Hex hex = get_hex(q, r);

		if (hex is null or not WfcHex)
			return;

		collapse_hex(hex as WfcHex, HexTypes.get_type_from_name(type_name));
	}



	/*
	 * Remove the hex from the shortlist and uncollapsed list
	 * Tell the hex to collapse
	 * Constrain the neighbors
	 * This is so messy it's not even funny
	 */
	public void collapse_hex(WfcHex hex, int type)
	{
		if (hex.is_collapsed())
			return;
		
		collapsed.Add(hex);
		not_collapsed.Remove(hex);
		shortlist.remove(hex);

		hex.collapse(type);

		// Constrain each neighbor
		Hex[] neighbors = get_hex_neighbors(hex);

		for (int i = 0; i < 6; i++)
		{
			if (neighbors[i] is null || neighbors[i] is not WfcHex neighbor || neighbor.is_collapsed())
				continue;

			// Keep track of this for update_or_insert()
			int previous_num = neighbor.get_constraint();

			// Constrain the adjacent edge on the neighbor, which is offset by 3
			neighbor.constrain_edge(i + 3, hex.get_edge_type(i));

			shortlist.update_or_insert(neighbor, previous_num);

			// Hex past neighbor
			Hex[] neighbor_neighbors = get_hex_neighbors(neighbor);

			if (neighbor_neighbors[i] is null or not WfcHex)
				continue;

			WfcHex second_neighbor = neighbor_neighbors[i] as WfcHex;

			if (second_neighbor.is_collapsed())
				continue;

			previous_num = second_neighbor.get_constraint();

			second_neighbor.constrain_edge(i + 3, neighbor.get_allowed_edge_types(i));

			shortlist.update_or_insert(second_neighbor, previous_num);
			
			for (int j = 4; j <= 8; j++)
				constrain_neighbor(j % 6);

			void constrain_neighbor(int edge)
			{

				if (neighbor_neighbors[edge] is null or not WfcHex)
					return;

				WfcHex neighbor_to_constrain = neighbor_neighbors[edge] as WfcHex;

				if (neighbor_to_constrain.is_collapsed())
					return;
				

				int previous_num = neighbor_to_constrain.get_constraint();

				neighbor_to_constrain.constrain_edge(edge + 3, neighbor.get_allowed_edge_types(edge));

				shortlist.update_or_insert(neighbor_to_constrain, previous_num);
			}
		}
	}


	/*
	 * Return the path between two hexes
	 * Tries to make as many straight lines as possible
	 * Uses a modified version of the A* alg
	 */
	public List<Hex> find_path(Hex start, Hex end)
	{
		start ??= get_hex(0, 0);

		if (end is null)
			return new();

		if (!HexTypes.is_type_traversable(start.get_terrain_type())
			|| !HexTypes.is_type_traversable(end.get_terrain_type()))
			return new();

		// Search from end to start
		PriorityQueue<Hex, int> frontier = new();
		frontier.Enqueue(end, 0);
		List<Hex> reached = new() { end };
		Dictionary<Hex, Hex> came_from = new() {{ end, null }};
		Dictionary<Hex, int> cost_so_far = new() {{ end, 0 }};
		Dictionary<Hex, bool> traversable_boundary = new() {{ end, true }};

		int searched = 0;



		while (frontier.Count > 0)
		{
			searched++;
			if (searched > 10000)
				return new();

			Hex current = frontier.Dequeue();

			if (current == start)
				break;
			
			foreach (Hex next in get_hex_neighbors(current))
			{
				if (next is null)
					continue;
				
				if (!HexTypes.is_type_traversable(next.get_terrain_type()))
				{
					traversable_boundary[current] = true;
					continue;
				}

				int new_cost = cost_so_far[current];
				if (!reached.Contains(next))
				{
					reached.Add(next);
					came_from.Add(next, current);
					traversable_boundary.Add(next, false);
					cost_so_far.Add(next, new_cost);
					frontier.Enqueue(next, new_cost + (int) next.get_world_distance(start));
				}
				else if (new_cost < cost_so_far[next])
				{
					came_from[next] = current;
					cost_so_far[next] = new_cost;
					traversable_boundary[next] = false;
					frontier.Enqueue(next, new_cost + (int) next.get_world_distance(start));
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
				current = came_from[current];

				if (traversable_boundary[current])
				{
					path.AddRange(get_hexes_on_line(line_start, current));
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
	private List<Hex> get_hexes_on_line(Hex start, Hex end)
	{
		start ??= end;

		if (end == null)
			return new();

		// If start == end we get a division by zero
		if (start == end)
			return new() {start};

		int dist = start.get_distance(end);
		List<Hex> line_hexes = new();

		for (int i = 0; i < dist + 1; i++)
		{
			Hex nearest_hex = Hex.lerp(start, end, (float) i / (float) dist);
			line_hexes.Add(get_hex(nearest_hex.get_q(), nearest_hex.get_r()));
		}

		return line_hexes;
	}



	public bool collapsed_all_hexes()
	{
		return (not_collapsed.Count == 0);
	}



	public new void clear()
	{
		size = 0;
		hexes = new();
		collapsed = new();

		not_collapsed = new();

		shortlist = new();
	}
}
