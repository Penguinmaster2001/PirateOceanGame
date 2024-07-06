
using System;
using System.Linq;



namespace HexModule;



[Serializable]
public class HexType : IEquatable<HexType>
{
	public string Name { get; internal set; }
	public int Weight { get; internal set; }
	public string MaterialPath { get; internal set; }
	// public Material HexMaterial { get; private set; }
	public EdgeType[] EdgeTypes { get; internal set; }
	private int edgeTypesHashCode = 0;
	public NavigableTag NavigableTag { get; internal set; }


	public static HexType Wildcard { get; }


	static HexType()
	{
		Wildcard = new();
	}



	public HexType(string name = null, int? weight = 0,
		string materialPath = null, EdgeType[] edgeTypes = null, NavigableTag? navigableTag = null)
	{
		Name = name ?? "Undefined";
		Weight = weight ?? 0;
		MaterialPath = materialPath ?? "res://TileTextures/undefined.tres";
		// HexMaterial = GD.Load<Material>(MaterialPath);
		EdgeTypes = edgeTypes ?? new EdgeType[6] { new(), new(), new(), new(), new(), new() };
		NavigableTag = navigableTag ?? new();
		RehashEdgeTypes();
	}



	internal void RehashEdgeTypes()
	{
		// edgeTypesHashCode = EdgeTypes.Aggregate(0, (acc, edge) => HashCode.Combine(acc, edge.GetHashCode()));

		for (int i = 0; i < Hex.NumEdges; i++)
		{
			edgeTypesHashCode = int.MinValue;
			edgeTypesHashCode += EdgeTypes[i].GetHashCode() * (83 ^ i);
		}
	}



	public HexType Clone()
	{
		return new HexType
		{
			Name = this.Name,
			Weight = this.Weight,
			MaterialPath = this.MaterialPath,
			EdgeTypes = this.EdgeTypes.ToArray(),
			NavigableTag = this.NavigableTag
		};
	}



	public bool Equals(HexType other)
	{
		if (other == null) return false;

		return this.GetHashCode() == other.GetHashCode()
			&& this.EdgeTypes.SequenceEqual(other.EdgeTypes);
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
		return HashCode.Combine(Name, edgeTypesHashCode);
	}



	public override string ToString()
	{
		return Name;
	}
}




[Serializable]
public struct EdgeType : IEquatable<EdgeType>
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



	public readonly bool Equals(EdgeType other)
	{
		return this.Type == other.Type;
	}



	public override readonly bool Equals(object obj)
	{
		if (obj is null or not EdgeType) return false;

		return this.Equals((EdgeType) obj);
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


[Serializable]
public struct NavigableTag
{
	public int Tag;



	public NavigableTag()
	{
		Tag = 0;
	}



	public NavigableTag(int tag)
	{
		Tag = tag;
	}
}
