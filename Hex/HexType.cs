
using System;

namespace Hex
{
	[Serializable]
	public class HexType
	{
		public string Name { get; protected set; }
		public int Weight { get; protected set; }
		public bool IsTraversable { get; protected set; }
		public string MaterialPath { get; protected set; }
		public int[] Edges { get; protected set; }



		public HexType(string name = null, int weight = 0, bool isTraversable = false,
			string materialPath = null, int[] edges = null)
		{
			Name = name ?? "Undefined";
			Weight = weight;
			IsTraversable = isTraversable;
			MaterialPath = materialPath ?? "res://TileTextures/undefined.tres";
			Edges = edges ?? new int[6] { 0, 0, 0, 0, 0, 0 };
		}



        public override string ToString()
        {
            return Name;
        }
    
	}
}
