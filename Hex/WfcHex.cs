using Godot;
using System.Collections.Generic;
using System.Linq;


namespace Hex
{
	public partial class WfcHex : Hex
	{
		public bool Collapsed { get; private set; }

		private int[] edgeTypes = new int[] { 0, 0, 0, 0, 0, 0 };
		public List<int> GetEdgeTypes() => edgeTypes.ToList();
		public int GetEdgeType(int edge) => edgeTypes[edge % 6];

		private List<HexType>[] allowedEdgeTypes = new List<HexType>[6];
		public List<HexType> GetAllowedEdgeTypes(int edge) => allowedEdgeTypes[edge % 6];

		public List<HexType> AllowedTypes { get; private set; }

		private int constraint = 0;
		public int GetConstraint() => constraint;

		public WfcHex(int q, int r) : base(q, r)
		{
			Collapsed = false;

			AllowedTypes = HexTypesCollection.hexTypes.ToList();

			// Fill all the lists
			for (int i = 0; i < 6; i++)
				allowedEdgeTypes[i] = AllowedTypes.ToList();

			// Update all the edges after filling the lists
			for (int i = 0; i < 6; i++)
				UpdateEdgeAllowedTypes(i);

			Constrain();
		}

		public void ConstrainEdge(int edge, int allowedEdgeType)
		{
			ConstrainEdge(edge, new List<int> { allowedEdgeType });
		}

		public void ConstrainEdge(int edge, List<int> allowedEdgeTypes)
		{
			edge %= 6;

			if (allowedEdgeTypes.Contains(0)) return;

			foreach (HexType allowedType in AllowedTypes.ToList())
			{
				int typeEdge = allowedType.Edges[edge];

				if (!allowedEdgeTypes.Contains(typeEdge))
					AllowedTypes.Remove(allowedType);
			}

			UpdateEdgeAllowedTypes(edge);

			Constrain();
		}

		private void UpdateEdgeAllowedTypes(int edge)
		{
			edge %= 6;

			List<int> edgeAllowedTypes = new();

			foreach (HexType allowedType in AllowedTypes)
			{
				int[] typeEdges = allowedType.Edges;

				if (!edgeAllowedTypes.Contains(typeEdges[edge]))
					edgeAllowedTypes.Add(typeEdges[edge]);
			}

			allowedEdgeTypes[edge] = edgeAllowedTypes;

			Constrain();
		}

		private void Constrain()
		{
			constraint = 0;

			int allowedTypeWeight = 0;
			foreach (int allowedType in AllowedTypes)
				allowedTypeWeight += HexTypes.GetTypeWeight(allowedType);

			int numAllowedEdges = 0;
			foreach (List<int> edge in allowedEdgeTypes)
			{
				foreach (int type in edge)
				{
					numAllowedEdges++;
				}
			}

			constraint = Mathf.Min(36 * allowedTypeWeight / 3, numAllowedEdges * 1000);
		}

		public int GetRandomAllowedType()
		{
			RandomNumberGenerator rng = new();

			if (AllowedTypes.Count == 0)
				return 0;

			int cumWeight = 0;
			List<int> cumWeights = new(AllowedTypes.Count);

			foreach (int allowedType in AllowedTypes)
			{
				cumWeight += HexTypes.GetTypeWeight(allowedType);
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

			return AllowedTypes[index];
		}

		public void Collapse(HexType type)
		{
			Collapsed = true;
			terrainType = type;
			edgeTypes = HexTypes.GetTypeEdges(type);
		}

		public override string ToString()
		{
			return "Wfc" + base.ToString();
		}
	}
}
