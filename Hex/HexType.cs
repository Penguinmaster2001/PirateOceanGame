
using System;
using Godot;

namespace HexModule
{
	[Serializable]
	public class HexType
	{
		public string Name { get; protected set; }
		public int Weight { get; protected set; }
		public bool IsTraversable { get; protected set; }
		public Material Material { get; protected set; }
		public EdgeType[] Edges { get; protected set; }


		public static HexType Wildcard { get; }



		static HexType()
		{
			Wildcard = new();
		}



		public HexType(string name = null, int weight = 0, bool isTraversable = false,
			string materialPath = null, EdgeType[] edges = null)
		{
			Name = name ?? "Undefined";
			Weight = weight;
			IsTraversable = isTraversable;
			Material = GD.Load<Material>(materialPath ?? "res://TileTextures/undefined.tres");
			Edges = edges ?? new EdgeType[6] { new(), new(), new(), new(), new(), new() };
		}



        public override string ToString()
        {
            return Name;
        }
	}



	/*
	 * This is really just meant to act as a type alias for int
	 * I may add more in the future, however
	 */
	public struct EdgeType
	{
		public int Type { get; set; }

		public static EdgeType Wildcard { get; }

		static EdgeType()
		{
			Wildcard = new();
		}

        public EdgeType(int type = 0)
        {
			Type = type;
        }


        public override readonly string ToString()
        {
            return Type.ToString();
        }
    }
}
