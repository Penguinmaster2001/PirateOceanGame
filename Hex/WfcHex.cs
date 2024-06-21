
using Godot;

using System.Linq;
using System.Collections.Generic;


namespace HexModule
{
	public partial class WfcHex : Hex
	{
		public bool Collapsed { get; private set; }

		private readonly HashSet<EdgeType>[] validEdgeTypes = new HashSet<EdgeType>[NumEdges];
		public HashSet<EdgeType> GetValidEdgeTypes(int edgeIndex) => validEdgeTypes[edgeIndex % NumEdges];

		public HashSet<HexType> ValidHexTypes { get; private set; }

		public int Constraint { get; private set; }



		public WfcHex(int q, int r) : base(q, r)
		{
			Collapsed = false;

			ValidHexTypes = HexTypesCollection.AllHexTypes.ToHashSet();

			// If we don't do this, every hex will have the wildcard and there will be no constraints
			ValidHexTypes.Remove(HexTypesCollection.AllHexTypes.ToArray()[0]);

			// Fill all the lists
			HashSet<EdgeType> commonEdges = HexTypesCollection.AllEdgeTypes;
			commonEdges.Remove(EdgeType.Wildcard);

			for (int i = 0; i < NumEdges; i++)
				validEdgeTypes[i] = commonEdges;

			Constrain();
		}



		public void ConstrainEdge(int edge, EdgeType allowedEdgeType)
		{
			ConstrainEdge(edge, new HashSet<EdgeType> { allowedEdgeType });
		}



		// Remove valid hex types that don't have their edge in the new valid edge types list
		public void ConstrainEdge(int edgeIndex, HashSet<EdgeType> newValidEdgeTypes)
		{
			edgeIndex %= NumEdges;

			if (newValidEdgeTypes.Contains(EdgeType.Wildcard)) return;

			HashSet<EdgeType> currentValidEdgeTypes = new HashSet<EdgeType>();
			List<HexType> toRemove = new List<HexType>();

			// Collect hex types to remove and update currentValidEdgeTypes in one pass
			foreach (HexType validHexType in ValidHexTypes)
			{
				EdgeType edgeType = validHexType.EdgeTypes[edgeIndex];
				if (!newValidEdgeTypes.Contains(edgeType))
				{
					toRemove.Add(validHexType);
				}
				else
				{
					currentValidEdgeTypes.Add(edgeType);
				}
			}

			// Remove invalid hex types in bulk
			foreach (HexType hexType in toRemove)
			{
				ValidHexTypes.Remove(hexType);
			}

			// Directly update validEdgeTypes for the edgeIndex
			validEdgeTypes[edgeIndex] = currentValidEdgeTypes;

			Constrain();
		}



		private void Constrain()
		{
			// Constraint = 0;

			// int allowedTypeWeight = 0;
			// foreach (HexType allowedType in ValidHexTypes)
			// 	allowedTypeWeight += allowedType.Weight;

			// int numAllowedEdges = validEdgeTypes.Sum(validEdgeType => validEdgeType.Count);

			Constraint = validEdgeTypes.Sum(validEdgeType => validEdgeType.Count); // Mathf.Min(36 * allowedTypeWeight / 3, numAllowedEdges * 1000);
		}



		public HexType GetRandomAllowedType()
		{
			RandomNumberGenerator rng = new();

			if (ValidHexTypes.Count == 0) return HexType.Wildcard;

			List<HexType> validHexTypesList = ValidHexTypes.ToList();

			int cumWeight = 0;
			List<int> cumWeights = new(validHexTypesList.Count);

			foreach (HexType allowedType in validHexTypesList)
			{
				cumWeight += allowedType.Weight;
				cumWeights.Add(cumWeight);
			}

			int choice = rng.RandiRange(0, cumWeight);

			int index = -1;
			foreach (int weight in cumWeights)
			{
				index++;

				if (weight > choice) break;
			}

			return validHexTypesList[index];
		}



		public void Collapse(HexType type)
		{
			Collapsed = true;
			TerrainType = type;

			// We need to update these lists in order for the neighboring hexes to be constrained
			ValidHexTypes = new() { type };

			for (int edgeIndex = 0; edgeIndex < NumEdges; edgeIndex++)
			{
				validEdgeTypes[edgeIndex] = new() { type.EdgeTypes[edgeIndex] };
			}
		}



		public override string ToString()
		{
			return "Wfc" + base.ToString() + "\t" + (Collapsed ? "Collapsed" : ("Constraint: " + Constraint));
		}
	}
}
