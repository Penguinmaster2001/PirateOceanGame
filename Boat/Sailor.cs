using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class Seaman
{
	private string name = "";

	private int age = 0;
	
	private SailorRole role;

	// Stats
	private int skill = 0;
	private int health = 100;


	public enum SailorRole
	{
		captain,
		navigator,
		carpenter,
		gunner,
		surgeon,
		purser,
		seaman,
		marine
	}


	private static List<string> first_names = new();
	private static List<string> last_names = new();

	private static RandomNumberGenerator rng = new();



	private static void read_names()
	{
		first_names.Add("Jack");
		last_names.Add("Sparrow");

		// Read from json
		FileAccess file = FileAccess.Open("res://Boat/SailorNames.json", FileAccess.ModeFlags.Read);
		string data = file.GetAsText();
		file.Close();

		Json json = new();
		json.Parse(data);

		// Ew
		Godot.Collections.Dictionary<string, Array<string>> json_dict
				= json.Data.AsGodotDictionary<string, Array<string>>();

		foreach (string first_name in json_dict["first_names"])
			first_names.Add(first_name);

		foreach (string last_name in json_dict["last_names"])
			last_names.Add(last_name);
	}



	// Make sure the names are read
	static Seaman()
	{
		read_names();
	}



	public Seaman()
	{
		name = last_names[rng.RandiRange(0, last_names.Count - 1)] + ", "
			+ first_names[rng.RandiRange(0, first_names.Count - 1)];

		age = rng.RandiRange(12, 45);

		role = (SailorRole) rng.RandiRange(0, 7);

		skill = rng.RandiRange(0, 100);
		health = rng.RandiRange(90, 100);
	}



    public override string ToString()
    {
        return name + ", aged " + age;
    }
}
