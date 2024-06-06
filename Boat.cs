using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Boat : Node3D
{
	private float travel_speed = 100.0f;

	private Hex cur_hex;
	private Hex next_hex;
	private Hex final_hex;
	private Vector3 target_pos;

	private Queue<Hex> travel_queue = new();

	private Node3D click_marker;
	private Node3D boat_model;



	public override void _Ready()
	{
		click_marker = GetParent().GetChild<Node3D>(2);
		boat_model = GetChild<Node3D>(1);
	}



	public override void _Process(double delta)
	{
		float deltaf = (float) delta;

		float dist_to_target = Position.DistanceTo(target_pos);
		Vector3 dir_to_target = Position.DirectionTo(target_pos);

		if (dist_to_target > 5.0f)
			Position += dir_to_target * travel_speed * Mathf.Clamp(dist_to_target / 10.0f, 0.25f, 3.0f) * deltaf;

		if (dist_to_target < 35.0f)
		{
			cur_hex = next_hex;

			if (travel_queue.Count <= 0)
				return;

			next_hex = travel_queue.Dequeue();
			target_pos = next_hex.get_world_coords();
			boat_model.Call("update_look_at_target", target_pos);
		}
	}



    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseButton eventMouseButton
			&& eventMouseButton.IsPressed()
			&& eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			Camera3D camera = GetViewport().GetCamera3D();

			Vector3 origin = camera.ProjectRayOrigin(eventMouseButton.Position);
			Vector3 direction = camera.ProjectRayNormal(eventMouseButton.Position);

			Vector3 intersection = origin - (direction * origin.Y / direction.Y);

			click_marker.Position = intersection;

			Hex target_hex = HexMap.get_hex_at_world_coords(intersection.X, intersection.Z);

			foreach (Hex hex in HexMap.find_path(final_hex, target_hex))
				travel_queue.Enqueue(hex);

			final_hex = target_hex;
		}
    }
}
