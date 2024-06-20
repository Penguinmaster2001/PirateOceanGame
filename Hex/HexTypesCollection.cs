
using System.Collections.Generic;
using System.Linq;

namespace HexModule
{
	public static class HexTypesCollection
	{
		public static HashSet<HexType> AllHexTypes { get; private set; }
		public static List<string> HexTypeMaterialFilePaths { get; private set; }

		static HexTypesCollection()
		{
			AllHexTypes = HexTypesFileParser.ParseJson("res://Hex/hex_type_data.json");
		}



		public static HashSet<HexType> GetTypesWithName(string name)
		{
			return AllHexTypes
				.Where(hexType => hexType.Name == name)
				.ToHashSet();
		}



		public static HashSet<EdgeType> GetCommonEdgeTypes(HashSet<HexType> hexTypes)
		{
			return hexTypes
				.SelectMany(hexType => hexType.EdgeTypes)
				.ToHashSet();
		}



		public static EdgeType[] IntArrayToEdgeTypeArray(int[] ints)
		{
			EdgeType[] edgeTypes = new EdgeType[Hex.NumEdges];

			for (int i = 0; i < Hex.NumEdges; i++)
			{
				edgeTypes[i] = new(ints[i]);
			}

			return edgeTypes;
		}
	
	
		// public static HexType TypeWithName(string name)
		// {
		// 	int index = type_names.IndexOf(name);
		// 	return index == -1 ? 0 : index;
		// }
	}
}
