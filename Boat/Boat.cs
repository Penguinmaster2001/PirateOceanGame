using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

// This needs to be split up into a navigation class and boat management class
// and maybe a Node3D class as well

public partial class Boat : Node3D
{
	// Navigation stuff
	private float travel_speed = 100.0f;
	private Hex cur_hex;
	private Hex next_hex;
	private Hex final_hex;
	private Queue<Hex> travel_queue = new();
	private Vector3 target_pos;
	private bool traveling = false;
	

	private Node3D boat_model;

	private Node3D selection_circle;

	private bool selected = false;


	// Boat management stuff
	private string name = "";

	private ResourceManager resources = new();


	RandomNumberGenerator rng = new();



	public override void _Ready()
	{
		boat_model = (Node3D) GetNode("BoatModel");
		selection_circle = (Node3D) GetNode("SelectionCircle");
		
		selection_circle.Visible = false;
		target_pos = Position;

		cur_hex = HexMap.get_hex_at_world_coords(target_pos.X, target_pos.Z);
		next_hex = cur_hex;
		final_hex = cur_hex;

		name = "Ship No. " + rng.RandiRange(1000, 9999);
	}



	public override void _Process(double delta)
	{
		navigate_to_target((float) delta);
	}



	public void on_selection()
	{
		selected = true;
		selection_circle.Visible = true;
	}



	public void on_selection_cleared()
	{
		selected = false;
		selection_circle.Visible = false;
	}



	// Navigation functions
	private void navigate_to_target(float delta)
	{
		float dist_to_target = Position.DistanceTo(target_pos);
		Vector3 dir_to_target = Position.DirectionTo(target_pos);

		if (dist_to_target > 5.0f)
			Position += dir_to_target * travel_speed * Mathf.Clamp(dist_to_target / 10.0f, 0.25f, 3.0f) * delta;

		if (dist_to_target < 35.0f && travel_queue.Count > 0)
		{
			cur_hex = next_hex;

			next_hex = travel_queue.Dequeue();
			target_pos = next_hex.get_world_coords()
				+ new Vector3(rng.RandfRange(-20.0f, 20.0f), 0.0f, rng.RandfRange(-20.0f, 20.0f));
			
			boat_model.Call("update_look_at_target", target_pos);
		}
	}



	public void add_waypoint(Hex waypoint)
	{
		if (!selected)
			return;

		foreach (Hex hex in HexMap.find_path(final_hex, waypoint))
			travel_queue.Enqueue(hex);

		final_hex = waypoint;
		traveling = true;
	}



    public override string ToString()
    {
        return "Name: " + name
			+ "\nLocation: " + Position.ToString()
			+ "\nCurrently: " + (traveling ? "Sailing to " + final_hex.ToString() : "Anchored at " + cur_hex.ToString())
			+ "\nResources:\n " + resources.ToString()
			+ "\n\n";
    }
}
