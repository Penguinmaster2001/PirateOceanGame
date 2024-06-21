
using Godot;

using System;


namespace HexModule
{
	/* 
	* 
	* 
	*/
	public partial class Hex : GodotObject, IEquatable<Hex> // To allow signals to carry Hexes
	{
		public const int NumEdges = 6;


		public int Q { get; private set; }

		public int R { get; private set; }

		public int S { get; private set; }

		
		public HexType TerrainType { get; protected set; }

		public const float Size = 25.0f;


		public Hex(int q, int r)
		{
			Q = q;
			R = r;
			S = -q - r;

			TerrainType = HexType.Wildcard;
		}


		public Vector3 WorldCoordinates()
		{
			float x = Size * ((Q * Mathf.Sqrt(3.0f)) + (R * Mathf.Sqrt(3.0f) / 2.0f));
			float z = Size * 1.5f * R;

			return new Vector3(x, 0.0f, z);
		}


		public static Hex WorldCoordsToHex(float x, float y)
		{
			float q_frac = ((x * Mathf.Sqrt(3.0f) / 3.0f) - (y / 3.0f)) / Size;
			float r_frac = y * 2.0f / 3.0f / Size;

			return RoundCoordinates(q_frac, r_frac);
		}



		public int HexMagnitude()
			=> (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;



		public static int GetHexDistance(Hex a, Hex b)
			=> (a - b).HexMagnitude();



		public static float GetWorldDistance(Hex a, Hex b)
			=> a.WorldCoordinates().DistanceTo(b.WorldCoordinates());



		public static Hex operator +(Hex a, Hex b)
			=> new(a.Q + b.Q, a.R + b.R);



		public static Hex operator -(Hex a, Hex b)
			=> new(a.Q - b.Q, a.R - b.R);



		public static Hex Lerp(Hex start, Hex end, float weight)
			=> RoundCoordinates(Mathf.Lerp(start.Q, end.Q, weight),
								Mathf.Lerp(start.R, end.R, weight));



		public static Hex RoundCoordinates(float q_frac, float r_frac)
		{
			int q = Mathf.RoundToInt(q_frac);
			int r = Mathf.RoundToInt(r_frac);
			int s = -q - r;

			float q_diff = Mathf.Abs(q - q_frac);
			float r_diff = Mathf.Abs(r - r_frac);
			float s_diff = Mathf.Abs(-q_frac - r_frac);

			if (q_diff > r_diff && q_diff > s_diff)
				q = -r - s;
			else if (r_diff > s_diff)
				r = -q - s;

			
			return new Hex(q, r);
		}



        public bool Equals(Hex other)
        {
			if (other is null) return false;

        	return this.Q == other.Q && this.R == other.R;
        }



		public override bool Equals(object obj)
		{
			if (obj is Hex other) return this.Equals(other);

			return false;
		}



        public static bool operator ==(Hex left, Hex right)
        {
			if (left is null) return right is null;

            return left.Equals(right);
        }



        public static bool operator !=(Hex left, Hex right)
        {
            return !(left == right);
        }



		// Most of the time the pair (Q, R) is unique for this hex
		// And if it isn't, the two hexes are in different maps and likely won't ever interact
		public override int GetHashCode()
        {
            return System.HashCode.Combine(Q, R);
        }



        public override string ToString()
		{
			return "Hex(" + Q + ", " + R + ")" + "\tType: " + TerrainType.Name;
		}
    }
}
