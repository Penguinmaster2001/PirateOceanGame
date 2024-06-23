using Godot;
using System.Collections.Generic;


namespace HexModule.Map
{
	public class HexContainer
	{
		protected int size = 0;
		protected Dictionary<(int, int), WfcHex> hexes = new();


		public enum MapShape
		{
			triangle,
			hexagon
		}


		public HexContainer(MapShape shape, int size)
		{
			switch (shape)
			{
				case MapShape.triangle:
					PopulateTriangleGrid(size);
					break;

				case MapShape.hexagon:
					PopulateHexagonalGrid(size);
					break;
			}
		}



		/*
		* Generate a map in a triangle shape
		* (0, 0) is the bottom point, (side_length, 0), (0, side_length) are the other corners
		*/
		protected void PopulateTriangleGrid(int side_length)
		{
			size = side_length;

			for (int q = 0; q < side_length; q++)
				for (int r = 0; r < side_length - q; r++)
					InsertHex(q, r);
		}



		protected void PopulateHexagonalGrid(int side_length)
		{
			size = side_length - 1;

			for (int q = -size; q <= size; q++)
			{
				int r1 = Mathf.Max(-size, -size - q);
				int r2 = Mathf.Min( size,  size - q);

				for (int r = r1; r <= r2; r++)
					InsertHex(q, r);
			}
		}
		


		// Add a hex to the map at coords
		public void InsertHex(int q, int r)
		{
			hexes.Add((q, r), new(q, r));
		}



		public WfcHex GetHex(int q, int r)
		{
			return hexes.TryGetValue((q, r), out WfcHex hex) ? hex : null;
		}



		// Return a list of the six neighbors to the hex, null if a neighbor doesn't exist
		internal WfcHex[] GetHexNeighbors(Hex hex)
		{
			Vector2I[] neighbor_dirs = new Vector2I[] {
				new( 1,  0), // Right
				new( 1, -1), // Up Right
				new( 0, -1), // Up Left
				new(-1,  0), // Left
				new(-1,  1), // Down Left
				new( 0,  1)  // Down Right
			};

			List<WfcHex> neighbors = new();

			foreach (Vector2I dir in neighbor_dirs)
				neighbors.Add(GetHex(hex.Q + dir.X, hex.R + dir.Y));

			return neighbors.ToArray();
		}



		public void Clear()
		{
			size = 0;
			hexes = new();
		}
	}
}
