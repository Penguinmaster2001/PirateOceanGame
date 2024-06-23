
using Godot;
using Godot.Collections;

using System.Collections.Generic;



namespace Entities.Seamen
{
    internal static class SeamenNames
    {
        private static readonly List<string> seamenFirstNames = new();
        private static readonly List<string> seamenSurnames = new();

        private static readonly RandomNumberGenerator rng = new();


        private static void LoadNamesFromFile()
        {
            // Ensure the lists are not empty
            seamenFirstNames.Add("Jack");
            seamenSurnames.Add("Sparrow");

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
                seamenFirstNames.Add(first_name);

            foreach (string last_name in json_dict["last_names"])
                seamenSurnames.Add(last_name);
        }



        // Make sure the names are read
        static SeamenNames()
        {
            LoadNamesFromFile();
        }



        internal static string GetName()
        {
            return seamenSurnames[rng.RandiRange(0, seamenSurnames.Count - 1)] + ", "
                + seamenFirstNames[rng.RandiRange(0, seamenFirstNames.Count - 1)];
        }
    }
}