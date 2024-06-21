using System.Collections.Generic;

public class CrewManager
{
	private List<Seaman> officers = new();
	private List<Seaman> seamen = new();

    private int crew_capacity = 20;

	public CrewManager()
	{
		for (int i = 0; i < 5; i++)
		{
			officers.Add(new());
		}

		for (int i = 0; i < crew_capacity; i++)
		{
			add_crew();
		}
	}



	public void add_crew()
	{
		seamen.Add(new());
	}



    public override string ToString()
    {
		string crew_string = "Crew:\n";

		crew_string += "\nOfficers:";

		foreach (Seaman officer in officers)
			crew_string += "\n" + officer.ToString();
		
		crew_string += "\nSeamen:";

		foreach (Seaman seaman in seamen)
			crew_string += "\n" + seaman.ToString();

        return crew_string;
    }
}
