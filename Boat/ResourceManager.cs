using Godot;
using System;

public class ResourceManager
{
	private SupplyManager supplies = new();

	private CrewManager crew = new();


	public ResourceManager()
	{

	}



	public override string ToString()
	{
		return crew.ToString() + "\n" + supplies.ToString();
	}
}
