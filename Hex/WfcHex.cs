using Godot;
using System.Collections.Generic;
using System.Linq;


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
			for (int i = 0; i < NumEdges; i++)
				validEdgeTypes[i] = HexTypesCollection.GetCommonEdgeTypes(ValidHexTypes);

			// Update all the edges after filling the lists
			for (int i = 0; i < NumEdges; i++)
				UpdateValidEdgeTypes(i);
		}



		public void ConstrainEdge(int edge, EdgeType allowedEdgeType)
		{
			ConstrainEdge(edge, new HashSet<EdgeType> { allowedEdgeType });
		}



		public void ConstrainEdge(int edgeIndex, HashSet<EdgeType> newValidEdgeTypes)
		{
			edgeIndex %= NumEdges;

			if (newValidEdgeTypes.Contains(EdgeType.Wildcard)) return;

			// Remove hex types where the edge type at edgeIndex is not in newValidEdgeTypes
			ValidHexTypes.RemoveWhere(validHexType =>
			{
				EdgeType edgeType = validHexType.EdgeTypes[edgeIndex];
				return !newValidEdgeTypes.Contains(edgeType);
			});

			UpdateValidEdgeTypes(edgeIndex);
		}



		private void UpdateValidEdgeTypes(int edgeIndex)
		{
			validEdgeTypes[edgeIndex] = ValidHexTypes
				.Select(validHexType => validHexType.EdgeTypes[edgeIndex])
				.ToHashSet();

			Constrain();
		}



		private void Constrain()
		{
			Constraint = 0;

			int allowedTypeWeight = 0;
			foreach (HexType allowedType in ValidHexTypes)
				allowedTypeWeight += allowedType.Weight;

			int numAllowedEdges = 0;
			foreach (HashSet<EdgeType> validEdgeType in validEdgeTypes)
				numAllowedEdges += validEdgeType.Count;

			Constraint = Mathf.Min(36 * allowedTypeWeight / 3, numAllowedEdges * 1000);
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
			return "Wfc" + base.ToString();
		}
	}
}
