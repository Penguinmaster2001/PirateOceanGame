using Godot;
using System.Collections.Generic;

public class HexShortList
{
	private Dictionary<int, List<WfcHex>> shortlist = new();

	private int num_hexes = 0;
	public int get_num_hexes() => num_hexes;

	private int least_option = int.MaxValue;


	public void update_or_insert(WfcHex hex, int previous_num)
	{
		// The hex is already in the correct spot
		if (hex.get_constraint() == previous_num)
			return;

		// Remove from previous spot
		if (shortlist.ContainsKey(previous_num) && shortlist[previous_num].Contains(hex))
		{
			shortlist[previous_num].Remove(hex);
			num_hexes--;

			if (previous_num == least_option && shortlist[previous_num].Count == 0)
				find_least_option();
		}

		// Insert into new spot
		int new_num = hex.get_constraint();
		if (!shortlist.ContainsKey(new_num))
			shortlist.Add(new_num, new());

		shortlist[new_num].Add(hex);
		num_hexes++;

		if (new_num < least_option)
			least_option = new_num;
	}



	public void remove(WfcHex hex)
	{
		var num_options = hex.get_constraint();

		if (shortlist.ContainsKey(num_options) && shortlist[num_options].Contains(hex))
		{
			shortlist[num_options].Remove(hex);
			num_hexes--;

			if (num_options == least_option && shortlist[num_options].Count == 0)
				find_least_option();
		}
	}



	public WfcHex get_most_constrained_random()
	{
		RandomNumberGenerator rng = new();

		return shortlist[least_option][rng.RandiRange(0, shortlist[least_option].Count - 1)];
	}



	// Find the smallest key with a non-empty list
	private void find_least_option()
	{
		least_option = int.MaxValue;
		foreach(int key in shortlist.Keys)
		{
			if (key < least_option && shortlist[key].Count > 0)
				least_option = key;
		}
	}



	public bool is_empty()
	{
		return num_hexes == 0;
	}
}

/*
func _to_string() -> String:
	var string := "Number of hexes: " + String.num_int64(num_hexes) + "\n"

	for option: int in current_options:
		string += String.num_int64(option) + ": " + shortlist[option].to_string() + "\n"

	return string

*/
