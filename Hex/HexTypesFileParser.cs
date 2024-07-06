
using Godot;

using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;



namespace HexModule;



public static class HexTypesFileParser
{
	public static HashSet<HexType> ParseJson(string jsonPath)
	{
		string timestampPath = Path.ChangeExtension(jsonPath, null) + "_timestamp";
		string serializedPath = Path.ChangeExtension(jsonPath, null) + "_serialized.json";

		ulong jsonLastWriteTime = Godot.FileAccess.GetModifiedTime(jsonPath);
		ulong recordedLastWriteTime = 0;

		// Make sure the serialized file and the timestamp file both exist
		if (Godot.FileAccess.FileExists(serializedPath) && Godot.FileAccess.FileExists(timestampPath))
		{
			using Godot.FileAccess timestampFileRead = Godot.FileAccess.Open(timestampPath, Godot.FileAccess.ModeFlags.Read);
			recordedLastWriteTime = timestampFileRead.Get64();
		}

		if (jsonLastWriteTime <= recordedLastWriteTime)
		{
			GD.Print("Attempting to use old serial");
			
			// JSON file has not been modified, read data from binary file
			try
			{
				return ReadFromSerialFile(serializedPath);
			}
			catch (System.Exception e)
			{
				// Continue on to making a new serial file on exception.
				GD.PushError("Failed to read old serial, creating new. " + e.ToString());
			}
		}

		GD.Print("Making new serial");

		// JSON file has been modified since last run, parse it and write data to binary file
		HashSet<HexType> hexTypes = GenerateHexTypes(jsonPath);

		WriteToSerialFile(hexTypes, serializedPath);

		// Update the timestamp file
		using Godot.FileAccess timestampFileWrite = Godot.FileAccess.Open(timestampPath, Godot.FileAccess.ModeFlags.Write);
		timestampFileWrite.Store64((ulong)Time.GetUnixTimeFromSystem());

		return hexTypes;
	}



	private static HashSet<HexType> GenerateHexTypes(string jsonPath)
	{
		HashSet<HexType> hexTypes = ParseJsonFile(jsonPath);

		HashSet<HexType> symmetricHexTypes = new();
		foreach (HexType hexType in hexTypes)
		{
			foreach (HexType symmetricHexType in GenerateSymmetries(hexType))
			{
				symmetricHexTypes.Add(symmetricHexType);
			}
		}

		return symmetricHexTypes;
	}



	private static HashSet<HexType> ParseJsonFile(string jsonPath)
	{
		HashSet<HexType> loadedHexTypes = new();

		string jsonData = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read).GetAsText();

		using JsonDocument jsonDocument = JsonDocument.Parse(jsonData);

		foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
		{
			JsonElement hexTypeData = property.Value;
			
			HexType newHexType = new(name: property.Name);
			
			if (hexTypeData.TryGetProperty("weight", out JsonElement weightElement))
				newHexType.Weight = weightElement.GetInt32() * 1000;

			if (hexTypeData.TryGetProperty("navigableTags", out JsonElement navigableTags))
				newHexType.NavigableTag = new(navigableTags.GetInt32());

			if (hexTypeData.TryGetProperty("material", out JsonElement materialElement))
				newHexType.MaterialPath = materialElement.GetString();

			if (hexTypeData.TryGetProperty("edges", out JsonElement edgesElement))
			{
				newHexType.EdgeTypes = HexTypesCollection.IntArrayToEdgeTypeArray(
					edgesElement
						.EnumerateArray()
						.Select(je => JsonSerializer.Deserialize<int>(je.GetRawText()))
						.ToArray());

				newHexType.RehashEdgeTypes();
			}
			

			loadedHexTypes.Add(newHexType);
		}

		return loadedHexTypes;
	}



	private static void WriteToSerialFile(HashSet<HexType> list, string filePath)
	{
		using Stream file = File.Create(ProjectSettings.GlobalizePath(filePath));
		JsonSerializer.Serialize(file, list);
	}



	private static HashSet<HexType> ReadFromSerialFile(string filePath)
	{
		using Stream stream = File.Open(ProjectSettings.GlobalizePath(filePath), FileMode.Open);
		return JsonSerializer.Deserialize<HashSet<HexType>>(stream);
	}



	/*
		* This permutes the edges according to the Dihedral Group of Order 6,
		* the symmetries of a hexagon
		* It ignores duplicate entries
		* It will result in a maximum of 12 unique edge arrays
		*/
	private static HashSet<HexType> GenerateSymmetries(HexType hexType)
	{
		HashSet<HexType> symmetries = new();

		for (int i = 0; i < 6; i++)
		{
			HexType rotated = RotateEdges(hexType, i);
			symmetries.Add(rotated);

			HexType reflected = ReflectEdges(rotated);
			symmetries.Add(reflected);
		}

		foreach (HexType symmetricHexType in symmetries)
		{
			symmetricHexType.Weight /= symmetries.Count;
		}

		return symmetries;
	}


	// Rotate the edges amount times, the array must have length 6
	private static HexType RotateEdges(HexType hexType, int amount)
	{
		EdgeType[] rotatedEdges = new EdgeType[6];
		for (int i = 0; i < 6; i++)
			rotatedEdges[i] = hexType.EdgeTypes[(i + amount) % 6];

		HexType rotatedHexType = hexType.Clone();
		rotatedHexType.EdgeTypes = rotatedEdges;
		
		return rotatedHexType;
	}


	// Reflect the edges across the (0, 3) axis, the array must have length 6
	private static HexType ReflectEdges(HexType hexType)
	{
		EdgeType[] reflectedEdges = (EdgeType[]) hexType.EdgeTypes.Clone();

		reflectedEdges[1] = hexType.EdgeTypes[5];
		reflectedEdges[5] = hexType.EdgeTypes[1];

		reflectedEdges[2] = hexType.EdgeTypes[4];
		reflectedEdges[4] = hexType.EdgeTypes[2];

		HexType reflectedHexType = hexType.Clone();
		reflectedHexType.EdgeTypes = reflectedEdges;
		
		return reflectedHexType;
	}
}
