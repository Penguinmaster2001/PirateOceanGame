
using System.Collections.Generic;

namespace HexModule
{
	public static class HexTypesCollection
	{
		public static List<HexType> hexTypes { get; private set; }
		public static List<string> hexTypeMaterialPaths { get; private set; }

		static HexTypesCollection()
		{
			hexTypes = HexTypesFileParser.ParseJson("res://Hex/hex_type_data.json");
		}



		public static List<EdgeType> GetCommonEdgeTypes(List<HexType> hexTypes)
		{
			List<EdgeType> common = new();

			foreach (HexType hexType in hexTypes)
			{
				foreach (EdgeType edgeType in hexType.Edges)
				{
					if (!common.Contains(edgeType))
						common.Add(edgeType);
				}
			}

			return common;
		}
	}
}
