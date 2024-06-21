using Godot;
using System.Collections.Generic;
using System.Linq;


namespace HexModule
{
	public class HexShortList
	{
		private class HexComparer : IComparer<WfcHex>
		{
			public int Compare(WfcHex x, WfcHex y)
			{
				if (x == y) return 0;

				int result = x.Constraint - y.Constraint;

				if (result == 0)
					result = x.Q - y.Q;

				if (result == 0)
					result = x.R - y.R;

				return result;
			}
		}



		private readonly SortedSet<WfcHex> hexShortlist;
		// private readonly RandomNumberGenerator rng = new();

		public int Count => hexShortlist.Count;



		public HexShortList()
		{
			hexShortlist = new SortedSet<WfcHex>(new HexComparer());
		}



		public void Insert(WfcHex hex)
		{
			hexShortlist.Add(hex);
			// GD.Print("Add: " + hexShortlist.Add(hex) + "\tSize: " + hexShortlist.Count + " Hex: " + hex.ToString());
		}



		public void Remove(WfcHex hex)
		{
			hexShortlist.Remove(hex);
			// GD.Print("Rem: " + hexShortlist.Remove(hex) + "\tSize: " + hexShortlist.Count + " Hex: " + hex.ToString());
		}



		public bool TryGetMostConstrainedHex(out WfcHex mostConstrainedHex)
		{
			mostConstrainedHex = null;

			if (IsEmpty()) return false;

			List<WfcHex> leastConstrainedHexes = hexShortlist
				.TakeWhile(hex => hex.Constraint == hexShortlist.Min.Constraint)
				.ToList();

			// GD.Print(hexShortlist.Count);
			// GD.Print(leastConstrainedHexes.Count);
			// GD.Print();

			// mostConstrainedHex = leastConstrainedHexes.ElementAt(rng.RandiRange(0, leastConstrainedHexes.Count - 1));
			// GD.Print("Random most constrained: " + randomMostConstrainedHex.ToString());

			mostConstrainedHex = hexShortlist.Min;

			if (mostConstrainedHex.Collapsed)
			{
				GD.PushError("ERROR: Most constrained is collapsed");

				// GD.Print("Removing bad hex" + randomMostConstrainedHex.ToString());
				// GD.Print(hexShortlist.Contains(randomMostConstrainedHex));
				// this.Remove(randomMostConstrainedHex);

				// randomMostConstrainedHex = null;
				
				// return false;
			}

			return true;
		}



		public bool IsEmpty()
		{
			return hexShortlist.Count == 0;
		}
	}
}
