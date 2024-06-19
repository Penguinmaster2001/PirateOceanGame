using Godot;
using System.Collections.Generic;
using System.Linq;


namespace HexModule
{
	public partial class WfcHex : Hex
	{
		public bool Collapsed { get; private set; }

		private readonly List<EdgeType>[] validEdgeTypes = new List<EdgeType>[6];
		public List<EdgeType> GetValidEdgeTypes(int edgeIndex) => validEdgeTypes[edgeIndex % 6];

		public List<HexType> ValidHexTypes { get; private set; }

		public int Constraint { get; private set; }



		public WfcHex(int q, int r) : base(q, r)
		{
			Collapsed = false;

			ValidHexTypes = HexTypesCollection.hexTypes.ToList();

			// Fill all the lists
			for (int i = 0; i < numEdges; i++)
				validEdgeTypes[i] = HexTypesCollection.GetCommonEdgeTypes(ValidHexTypes);

			// Update all the edges after filling the lists
			for (int i = 0; i < numEdges; i++)
				UpdateEdgeAllowedTypes(i);

			Constrain();
		}



		public void ConstrainEdge(int edge, EdgeType allowedEdgeType)
		{
			ConstrainEdge(edge, new List<EdgeType> { allowedEdgeType });
		}



		public void ConstrainEdge(int edge, List<EdgeType> allowedEdgeTypes)
		{
			edge %= numEdges;

			if (allowedEdgeTypes.Contains(EdgeType.Wildcard)) return;

			foreach (HexType allowedType in ValidHexTypes.ToList())
			{
				EdgeType typeEdge = allowedType.Edges[edge];

				if (!allowedEdgeTypes.Contains(typeEdge))
					ValidHexTypes.Remove(allowedType);
			}

			UpdateEdgeAllowedTypes(edge);

			Constrain();
		}



		private void UpdateEdgeAllowedTypes(int edge)
		{
			edge %= numEdges;

			List<EdgeType> edgeAllowedTypes = new();

			foreach (HexType validHexType in ValidHexTypes)
			{
				EdgeType[] typeEdges = validHexType.Edges;

				if (!edgeAllowedTypes.Contains(typeEdges[edge]))
					edgeAllowedTypes.Add(typeEdges[edge]);
			}

			validEdgeTypes[edge] = edgeAllowedTypes;

			Constrain();
		}



		private void Constrain()
		{
			Constraint = 0;

			int allowedTypeWeight = 0;
			foreach (HexType allowedType in ValidHexTypes)
				allowedTypeWeight += allowedType.Weight;

			int numAllowedEdges = 0;
			foreach (List<EdgeType> validEdgeType in validEdgeTypes)
				numAllowedEdges += validEdgeType.Count;

			Constraint = Mathf.Min(36 * allowedTypeWeight / 3, numAllowedEdges * 1000);
		}



		public HexType GetRandomAllowedType()
		{
			RandomNumberGenerator rng = new();

			if (ValidHexTypes.Count == 0) return new();

			int cumWeight = 0;
			List<int> cumWeights = new(ValidHexTypes.Count);

			foreach (HexType allowedType in ValidHexTypes)
			{
				cumWeight += allowedType.Weight;
				cumWeights.Add(cumWeight);
			}

			int choice = rng.RandiRange(0, cumWeight);

			int index = -1;
			foreach (int weight in cumWeights)
			{
				index++;

				if (weight > choice)
					break;
			}

			return ValidHexTypes[index];
		}



		public void Collapse(HexType type)
		{
			Collapsed = true;
			TerrainType = type;
		}



		public override string ToString()
		{
			return "Wfc" + base.ToString();
		}
	}
}
