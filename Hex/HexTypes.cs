using Godot;
using System.Collections.Generic;
using System.Linq;

public static class HexTypes
{
	// As more of these get added, I'm going to have to hold them into a data structure
	private static List<int> all_types = new();
	public  static List<int> get_all_types() => all_types;

	private static List<List<int>> base_to_types = new();
	private static List<int> types_to_base = new();

	private static List<string>	type_names = new();
	public static string get_name(int type) => type_names[type];

	private static List<int[]> type_edges = new();

	private static List<int> type_weights = new();

	private static List<string> type_materials = new();
	public static List<string> get_type_material_paths() => type_materials;
	
	private static List<bool> type_traversable = new();
	public static bool is_type_traversable(int type) => type_traversable[type];

	private static int num_types = 0;

	private const string hex_type_data_json_path = "res://Hex/hex_type_data.json";


	static HexTypes()
	{
		// Read from json
		FileAccess file = FileAccess.Open(hex_type_data_json_path, FileAccess.ModeFlags.Read);
		string data = file.GetAsText();
		file.Close();

		Json json = new();
		json.Parse(data);

		// Ew
		Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> json_dict
				= json.Data.AsGodotDictionary<string, Godot.Collections.Dictionary<string, Variant>>();

		List<string> names_from_json = json_dict.Keys.ToList();
		List<int[]>  edges_from_json = new();
		List<int> 	 weights_from_json = new();
		List<string> materials_from_json = new();
		List<bool> 	 traversable_from_json = new();

		int num_json_types = 0;


		foreach (Godot.Collections.Dictionary<string, Variant> hex_type_data in json_dict.Values)
		{
			edges_from_json.Add((int[]) hex_type_data["edges"]);
			weights_from_json.Add((int) hex_type_data["weight"]);
			materials_from_json.Add((string) hex_type_data["material"]);

			if(hex_type_data.TryGetValue("traversable", out Variant is_traversable))
				traversable_from_json.Add((bool) is_traversable);
			else
				traversable_from_json.Add(false);

			num_json_types++;
		}


		for (int i = 0; i < num_json_types; i++)
		{
			List<int[]> symmetric_edge_arrays = generate_symmetries(edges_from_json[i], out int num_symmetries);
			type_edges.AddRange(symmetric_edge_arrays);

			base_to_types.Add(new());

			for (int j = 0; j < num_symmetries; j++)
			{
				all_types.Add(num_types);
				base_to_types[i].Add(num_types);
				types_to_base.Add(i);

				num_types++;

				type_names.Add(names_from_json[i]);
				type_weights.Add(1000 * weights_from_json[i] / num_symmetries);
				type_materials.Add(materials_from_json[i]);
				type_traversable.Add(traversable_from_json[i]);
			}
		}
	}



	/*
	 * This permutes the edges according to the Dihedral Group of Order 6,
	 * the symmetries of a hexagon
	 * It ignores duplicate entries
	 * It will result in a maximum of 12 unique edge arrays
	 */
	private static List<int[]> generate_symmetries(int[] edge_types, out int num_symmetries)
	{
		List<int[]> symmetries = new()
        {
            edge_types
        };

		for (int i = 1; i < 6; i++)
		{
			int[] rotated = rotate_edges(edge_types, i);
			if (!symmetries.Contains(rotated))
				symmetries.Add(rotated);

			int[] reflected = reflect_edges(rotated);
			if (!symmetries.Contains(reflected))
				symmetries.Add(reflected);
		}

		num_symmetries = symmetries.Count;
		return symmetries;
	}


	// Rotate the edges amount times, the array must have length 6
	private static int[] rotate_edges(int[] edges, int amount)
	{
		int[] rotated_edges = new int[6];
		for (int i = 0; i < 6; i++)
			rotated_edges[i] = edges[(i + amount) % 6];
		
		return rotated_edges;
	}


	// Reflect the edges across the (0, 3) axis, the array must have length 6
	private static int[] reflect_edges(int [] edges)
	{
		int[] reflected_edges = (int[]) edges.Clone();

		reflected_edges[1] = edges[5];
		reflected_edges[5] = edges[1];

		reflected_edges[2] = edges[4];
		reflected_edges[4] = edges[2];

		return reflected_edges;
	}



	public static int get_type_weight(int type)
	{
		if (type_weights.Count <= type || type < 0)
			return 0;

		return type_weights[type];
	}


	public static int[] get_type_edges(int type)
	{
		if (type_edges.Count <= type || type < 0)
		{
			GD.Print(type_edges.Count);
			GD.PushWarning(type);

			return new int[] {0, 0, 0, 0, 0, 0};
		}

		return type_edges[type];
	}


	public static int get_type_from_name(string name)
	{
		int index = type_names.IndexOf(name);
		return index == -1 ? 0 : index;
	}
}
