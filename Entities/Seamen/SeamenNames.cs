
using Godot;
using Godot.Collections;

using System.Collections.Generic;



namespace Entities.Seamen;



/// <summary>
/// Reads lists of names from file and generates random names for new seamen.
/// </summary>
internal static class SeamenNames
{
    private static readonly List<string> seamenFirstNames = new();
    private static readonly List<string> seamenSurnames = new();

    private static readonly RandomNumberGenerator rng = new();


    /// <summary>
    /// Load the lists of names from the file.
    /// One first name and one last name is hardcoded so the lists won't be empty
    /// even if there's an error reading the file.
    /// </summary>
    private static void LoadNamesFromFile()
    {
        // Ensure the lists are not empty
        seamenFirstNames.Add("Jack");
        seamenSurnames.Add("Sparrow");

        // Read from json
        using FileAccess file = FileAccess.Open("res://Boat/SailorNames.json", FileAccess.ModeFlags.Read);

        // If the file was not successfully read, push error and return
        if (file is null)
        {
            GD.PushError("Sailor names file not opened: " + FileAccess.GetOpenError().ToString());

            return;
        }

        string data = file.GetAsText();

        Json json = new();
        json.Parse(data);

        // Ew
        Godot.Collections.Dictionary<string, Array<string>> json_dict
                = json.Data.AsGodotDictionary<string, Array<string>>();

        // Add the names to the list
        seamenFirstNames.AddRange(json_dict["first_names"]);

        seamenSurnames.AddRange(json_dict["last_names"]);
    }



    /// <summary>
    /// Load the names from file
    /// </summary>
    static SeamenNames()
    {
        LoadNamesFromFile();
    }



    /// <summary>
    /// Randomly generate a name for a seaman in the form "Surname, First Name"
    /// </summary>
    /// <returns>
    /// string that holds the randomly generated name.
    /// </returns>
    internal static string GetName()
    {
        return seamenSurnames[rng.RandiRange(0, seamenSurnames.Count - 1)] + ", "
            + seamenFirstNames[rng.RandiRange(0, seamenFirstNames.Count - 1)];
    }
}
