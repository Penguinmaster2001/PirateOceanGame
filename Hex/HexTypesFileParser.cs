
using System.Collections.Generic;

using System.IO;
using System.Text.Json;


namespace Hex
{
	public static class HexTypesFileParser
	{
		// private const string hexTypeDataJsonPath = "res://Hex/hex_type_data.json";
		// private const string hexTypeDataBinPath = "res://Hex/hex_type_data.bin";
		// private const string hexTypeDataTimestampPath = "res://Hex/hex_type_data_timestamp.txt";


		public static List<HexType> ParseJson(string jsonPath)
		{
			return ReadFromSerialFile(jsonPath);
			// DateTime lastWriteTimeJson = File.GetLastWriteTime(hexTypeDataJsonPath);
			// DateTime lastWriteTimeRecorded = DateTime.MinValue;

			// if (File.Exists(hexTypeDataTimestampPath))
			// {
			// 	lastWriteTimeRecorded = DateTime.Parse(File.ReadAllText(hexTypeDataTimestampPath));
			// }

			// if (lastWriteTimeJson > lastWriteTimeRecorded)
			// {
			// 	// JSON file has been modified since last run, parse it and write data to binary file
			// 	List<HexType> hexTypes = ParseJsonFile(hexTypeDataJsonPath);
			// 	WriteToSerialFile(hexTypes, hexTypeDataBinPath);

			// 	// Update the timestamp file
			// 	File.WriteAllText(hexTypeDataTimestampPath, lastWriteTimeJson.ToString());

			// 	HexTypes = hexTypes;
			// }
			// else
			// {
			// 	// JSON file has not been modified, read data from binary file
			// 	HexTypes = ReadFromSerialFile(hexTypeDataBinPath);
			// }
		}

		// private static List<HexType> ParseJsonFile(string filePath)
		// {
		// 	// Read from json
		// 	FileAccess file = FileAccess.Open(hex_type_data_json_path, FileAccess.ModeFlags.Read);
		// 	string data = file.GetAsText();
		// 	file.Close();
	
		// 	Json json = new();
		// 	json.Parse(data);
	
		// 	// Ew
		// 	Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> json_dict
		// 			= json.Data.AsGodotDictionary<string, Godot.Collections.Dictionary<string, Variant>>();
	
		// 	List<string> names_from_json = json_dict.Keys.ToList();
		// 	List<int[]>  edges_from_json = new();
		// 	List<int> 	 weights_from_json = new();
		// 	List<string> materials_from_json = new();
		// 	List<bool> 	 traversable_from_json = new();
	
		// 	int json_type = 0;
		// 	foreach (Godot.Collections.Dictionary<string, Variant> hex_type_data in json_dict.Values)
		// 	{
		// 		edges_from_json.Add((int[]) hex_type_data["edges"]);
		// 		weights_from_json.Add((int) hex_type_data["weight"]);
		// 		materials_from_json.Add((string) hex_type_data["material"]);
	
		// 		if(hex_type_data.TryGetValue("traversable", out Variant is_traversable))
		// 			traversable_from_json.Add((bool) is_traversable);
		// 		else
		// 			traversable_from_json.Add(false);
	
		// 		List<int[]> symmetric_edge_arrays = generate_symmetries(edges_from_json[json_type], out int num_symmetries);
		// 		type_edges.AddRange(symmetric_edge_arrays);
	
		// 		base_to_types.Add(new());
	
		// 		for (int j = 0; j < num_symmetries; j++)
		// 		{
		// 			all_types.Add(num_types);
		// 			base_to_types[json_type].Add(num_types);
		// 			types_to_base.Add(json_type);
	
		// 			num_types++;
	
		// 			type_names.Add(names_from_json[json_type]);
		// 			type_weights.Add(1000 * weights_from_json[json_type] / num_symmetries);
		// 			type_materials.Add(materials_from_json[json_type]);
		// 			type_traversable.Add(traversable_from_json[json_type]);
		// 		}
	
		// 		json_type++;
		// 	}

		// 	return new();
		// }


		// private static void WriteToSerialFile(List<HexType> list, string filePath)
		// {
        //     using Stream stream = File.Open(filePath, FileMode.Create);
        //     JsonSerializer.Serialize(stream, list);
        // }

		private static List<HexType> ReadFromSerialFile(string filePath)
		{
            using Stream stream = File.Open(filePath, FileMode.Open);
            return JsonSerializer.Deserialize<List<HexType>>(stream);
        }
	
	
	
		/*
		* This permutes the edges according to the Dihedral Group of Order 6,
		* the symmetries of a hexagon
		* It ignores duplicate entries
		* It will result in a maximum of 12 unique edge arrays
		*/
		private static List<int[]> generate_symmetries(int[] edge_types, out int num_symmetries)
		{
			List<int[]> symmetries = new();
	
			for (int i = 0; i < 6; i++)
			{
				int[] rotated = rotate_edges(edge_types, i);
				if (!already_added(rotated))
					symmetries.Add(rotated);
	
				int[] reflected = reflect_edges(rotated);
				if (!already_added(reflected))
					symmetries.Add(reflected);
			}
	
			num_symmetries = symmetries.Count;
	
			return symmetries;
	
			bool already_added(int[] to_check)
			{
				foreach (int[] arr in symmetries)
				{
					bool identical = true;
					for (int i = 0; i < 6; i++)
					{
						if (arr[i] != to_check[i])
						{
							identical = false;
							break;
						}
					}
	
					if (identical) return true;
				}
	
				return false;
			}
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
	
	
	
		// public static int get_type_weight(int type)
		// {
		// 	if (type_weights.Count <= type || type < 0)
		// 		return 0;
	
		// 	return type_weights[type];
		// }
	
	
		// public static int[] get_type_edges(int type)
		// {
		// 	if (type_edges.Count <= type || type < 0)
		// 	{
		// 		GD.Print(type_edges.Count);
		// 		GD.PushWarning(type);
	
		// 		return new int[] {0, 0, 0, 0, 0, 0};
		// 	}
	
		// 	return type_edges[type];
		// }
	
	
		// public static int get_type_from_name(string name)
		// {
		// 	int index = type_names.IndexOf(name);
		// 	return index == -1 ? 0 : index;
		// }
	}
}
	