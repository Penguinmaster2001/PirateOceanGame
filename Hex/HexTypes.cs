
using System.Collections.Generic;

namespace Hex
{
	public static class HexTypesCollection
	{
		public static List<HexType> hexTypes { get; private set; }

		static HexTypesCollection()
		{
			hexTypes = HexTypesFileParser.ParseJson("res://Hex/hex_type_data.json");
		}
	}
}
