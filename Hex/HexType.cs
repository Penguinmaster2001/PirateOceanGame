
using System;
using System.Linq;
using Godot;

namespace HexModule
{
	[Serializable]
	public class HexType : IEquatable<HexType>
	{
		public string Name { get; internal set; }
		public int Weight { get; internal set; }
		public bool Traversable { get; internal set; }
		public string MaterialPath { get; internal set; }
        public EdgeType[] EdgeTypes { get; internal set; }
		private int edgeTypesHashCode = 0;


        public static HexType Wildcard { get; }


        static HexType()
		{
			Wildcard = new();
		}



		public HexType(string name = null, int weight = 0, bool traversable = false,
			string materialPath = null, EdgeType[] edgeTypes = null)
		{
			Name = name ?? "Undefined";
			Weight = weight;
			Traversable = traversable;
			MaterialPath = materialPath ?? "res://TileTextures/undefined.tres";
			RehashEdgeTypes(edgeTypes ?? new EdgeType[6] { new(), new(), new(), new(), new(), new() });
		}



		private void RehashEdgeTypes(EdgeType[] newEdgeTypes)
		{
			edgeTypesHashCode = newEdgeTypes.Aggregate(0, (acc, edge) => HashCode.Combine(acc, edge.GetHashCode()));
			EdgeTypes = newEdgeTypes;
		}



		public HexType Clone()
		{
			return new HexType
			{
				Name = this.Name,
				Weight = this.Weight,
				Traversable = this.Traversable,
				MaterialPath = this.MaterialPath,
				EdgeTypes = (EdgeType[]) this.EdgeTypes.Clone()
			};
		}



        public bool Equals(HexType other)
        {
			if (other == null) return false;

			if (!(this.Traversable == other.Traversable
				&& this.Weight == other.Weight
				&& this.Name == other.Name
				&& this.MaterialPath == other.MaterialPath))
					return false;

			for (int i = 0; i < Hex.NumEdges; i++)
			{
				if (this.EdgeTypes[i] != other.EdgeTypes[i]) return false;
			}

			return true;

			// this.EdgeTypes.SequenceEqual(other.EdgeTypes);
        }



		public override bool Equals(object obj)
		{
			if (obj is HexType other) return this.Equals(other);

			return false;
		}



        public static bool operator ==(HexType left, HexType right)
        {
			if (left is null) return right is null;

            return left.Equals(right);
        }



        public static bool operator !=(HexType left, HexType right)
        {
            return !(left == right);
        }
		
		

		public override int GetHashCode()
		{
			return HashCode.Combine(
				Name,
				Weight,
				Traversable,
				MaterialPath,
				edgeTypesHashCode);
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
	[Serializable]
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



        public override readonly bool Equals(object obj)
        {
			if (obj is null or not EdgeType) return false;

			return this.Type == ((EdgeType) obj).Type;
        }



        public static bool operator ==(EdgeType left, EdgeType right)
        {
            return left.Equals(right);
        }



        public static bool operator !=(EdgeType left, EdgeType right)
        {
            return !(left == right);
        }
		
		

		public override readonly int GetHashCode()
		{
			return Type.GetHashCode();
		}



        public override readonly string ToString()
        {
            return Type.ToString();
        }
    }
}
