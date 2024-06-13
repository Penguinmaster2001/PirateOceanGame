using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;

public partial class WfcHex : Hex
{
	private bool collapsed = false;
	public bool is_collapsed() => collapsed;

	private int[] edge_types = new int[] {0, 0, 0, 0, 0, 0};
    public int get_edge_type(int edge) => edge_types[edge % 6];

	private List<int>[] allowed_edge_types = new List<int>[6];
    public List<int> get_allowed_edge_types(int edge) => allowed_edge_types[edge % 6];
	
	private List<int> allowed_types = new();

	private int constraint = 0;
    public int get_constraint() => constraint;


    public WfcHex(int q, int r) : base(q, r)
	{
		collapsed = false;

		allowed_types = HexTypes.get_all_types().ToList();

		// Fill all the lists
		for (int i = 0; i < 6; i++)
			allowed_edge_types[i] = allowed_types.ToList();

		// Update all the edges after filling the lists
		for (int i = 0; i < 6; i++)
			update_edge_allowed_types(i);

		constrain();
	}



	public void constrain_edge(int edge, int allowed_edge_type)
	{
		constrain_edge(edge, new List<int> { allowed_edge_type });
	}


	public void constrain_edge(int edge, List<int> allowed_edge_types)
	{
		edge %= 6;

		if (allowed_edge_types.Contains(0)) return;

		foreach (int allowed_type in allowed_types.ToList())
		{
			int type_edge = HexTypes.get_type_edges(allowed_type)[edge];

			if (!allowed_edge_types.Contains(type_edge))
				allowed_types.Remove(allowed_type);
		}

		update_edge_allowed_types(edge);

		constrain();
	}



	private void update_edge_allowed_types(int edge)
	{
		edge %= 6;

		List<int> edge_allowed_types = new();

		foreach (int allowed_type in allowed_types)
		{
			int[] type_edges = HexTypes.get_type_edges(allowed_type);

			if (!edge_allowed_types.Contains(type_edges[edge]))
				edge_allowed_types.Add(type_edges[edge]);
		}

		allowed_edge_types[edge] = edge_allowed_types;

		constrain();
	}



	private void constrain()
	{
        constraint = 0;

		int allowed_type_weight = 0;
		foreach (int allowed_type in allowed_types)
			allowed_type_weight += HexTypes.get_type_weight(allowed_type);

		int num_allowed_edges = 0;
		foreach (List<int> edge in allowed_edge_types)
		{
			foreach (int type in edge)
			{
				num_allowed_edges++;
			}
		}

		constraint = Mathf.Min(36 * allowed_type_weight / 3, num_allowed_edges * 1000);
	}



	public int get_random_allowed_type()
	{
		RandomNumberGenerator rng = new();

		if (allowed_types.Count == 0)
			return 0;

		
		int cum_weight = 0;
		List<int> cum_weights = new(allowed_types.Count);

		foreach (int allowed_type in allowed_types)
		{
			cum_weight += HexTypes.get_type_weight(allowed_type);
			cum_weights.Add(cum_weight);
		}

		int choice = rng.RandiRange(0, cum_weight);

		int index = -1;
		foreach (int weight in cum_weights)
		{
			index++;

			if (weight > choice)
				break;
		}

		return allowed_types[index];
	}



	public void collapse(int type)
	{
		collapsed = true;
		terrain_type = type;
		edge_types = HexTypes.get_type_edges(type);
	}



    public override string ToString()
    {
        return "Wfc" + base.ToString();
    }

    /*

func set_allowed_types(new_allowed_types: Array) -> void:
	_allowed_types = new_allowed_types.duplicate()

	_allowed_edge_types.clear()

	for i in range(6):
		_allowed_edge_types.append([0, -2, -1, 1, 2, 3])

	_constrain()

		# _update_edge_allowed_types(i)



func _to_string() -> String:
	# var allowed_type_names := [ ]

	# for type: int in _allowed_types:
	# 	allowed_type_names.append(HexMap.type_names[type])

	# return "\n---\nWfcHex(_q: %d, _r: %d)\n%s\n%s\n%s\n%s\n" % [self._q, self._r, self._allowed_types, allowed_type_names, self._collapsed, HexMap.type_names[self.terrain_type]]
	# return "\n---\nWfcHex(_q: %d, _r: %d)\n%s\n%s\n%s\n" % [self._q, self._r, self._allowed_edge_types, self._collapsed, HexMap.type_names[self.terrain_type]]
	return "WfcHex(q: %d, r: %d, traversable: %s)" % [self._q, self._r, HexMap.type_traversable[self.terrain_type]]
	*/
}
