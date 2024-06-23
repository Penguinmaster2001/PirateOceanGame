using Godot;
using HexModule;
using HexModule.Map;
using System.Collections.Generic;

// The navigation stuff needs to go into its own class

public partial class Boat : Node3D
{
	// Navigation fields
	private float travel_speed = 100.0f;
	private Hex cur_hex;
	private Hex next_hex;
	private Hex final_hex;
	private Queue<Hex> travel_queue = new();
	private Vector3 target_pos;
	private bool traveling = false;

	public HexMap hex_map;
	

	private Node3D boat_model;

	private Node3D selection_circle;

	private bool selected = false;


	// Boat management stuff
	private string name = "";

	private ResourceManager resources = new();

	private RandomNumberGenerator rng = new();



	public override void _Ready()
	{
		boat_model = GetNode<Node3D>("BoatModel");
		selection_circle = GetNode<Node3D>("SelectionCircle");
		
		selection_circle.Visible = false;
		target_pos = Position;

		cur_hex = hex_map.HexAtWorldCoordinates(target_pos.X, target_pos.Z);
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
			target_pos = next_hex.WorldCoordinates()
				+ new Vector3(rng.RandfRange(-20.0f, 20.0f), 0.0f, rng.RandfRange(-20.0f, 20.0f));
			
			boat_model.Call("update_look_at_target", target_pos);
		}
	}



	public void add_waypoint(Hex waypoint)
	{
		if (!selected) return;

		foreach (Hex hex in hex_map.FindPathBetween(final_hex, waypoint))
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
