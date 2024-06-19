using Godot;
using System.Collections.Generic;


namespace HexModule
{
	public class HexShortList
	{
		private readonly Dictionary<int, List<WfcHex>> hexShortlist;

		public int HexCount { get; private set; }

		private int minimumHexConstraint;



		public HexShortList()
		{
			HexCount = 0;
			hexShortlist = new();
			minimumHexConstraint = int.MaxValue;
		}


		public void UpdateOrInsert(WfcHex hex, int previousConstraint)
		{
			// The hex is already in the correct spot
			if (hex.Constraint == previousConstraint)
				return;

			// Remove from previous spot
			if (hexShortlist.ContainsKey(previousConstraint) && hexShortlist[previousConstraint].Contains(hex))
			{
				hexShortlist[previousConstraint].Remove(hex);
				HexCount--;

				if (previousConstraint == minimumHexConstraint && hexShortlist[previousConstraint].Count == 0)
					GetLeastOption();
			}

			// Insert into new spot
			int newConstraint = hex.Constraint;
			if (!hexShortlist.ContainsKey(newConstraint))
				hexShortlist.Add(newConstraint, new());

			hexShortlist[newConstraint].Add(hex);
			HexCount++;

			if (newConstraint < minimumHexConstraint)
				minimumHexConstraint = newConstraint;
		}



		public void Remove(WfcHex hex)
		{
			var num_options = hex.Constraint;

			if (hexShortlist.ContainsKey(num_options) && hexShortlist[num_options].Contains(hex))
			{
				hexShortlist[num_options].Remove(hex);
				HexCount--;

				if (num_options == minimumHexConstraint && hexShortlist[num_options].Count == 0)
					GetLeastOption();
			}
		}



		public WfcHex GetRandomMostConstrained()
		{
			RandomNumberGenerator rng = new();

			return hexShortlist[minimumHexConstraint][rng.RandiRange(0, hexShortlist[minimumHexConstraint].Count - 1)];
		}



		// Find the smallest key with a non-empty list
		private void GetLeastOption()
		{
			minimumHexConstraint = int.MaxValue;
			foreach(int key in hexShortlist.Keys)
			{
				if (key < minimumHexConstraint && hexShortlist[key].Count > 0)
					minimumHexConstraint = key;
			}
		}



		public bool IsEmpty()
		{
			return HexCount == 0;
		}
	}
}
