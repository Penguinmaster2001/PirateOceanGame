using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class FleetController : Node3D
{
	[Signal]
	public delegate void AddedNewWaypointEventHandler(Vector3 waypoint);

	[Signal]
	public delegate void SelectedBoatsEventHandler(Array<Node3D> boats_selected);

	[Signal]
	public delegate void SelectionClearedEventHandler();


	private PackedScene boat_scene = (PackedScene) GD.Load("res://Boat/boat.tscn");

	private List<Boat> boats = new();
	private List<Boat> selected_boats = new();

	private float movement_speed = 1000.0f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}



	public override void _Process(double delta)
	{
		handle_input((float) delta);
	}



	private void add_new_boat()
	{
		Vector3 new_boat_coords = Position;
		new_boat_coords.Y = 0;

		Boat new_boat = (Boat) boat_scene.Instantiate();
		new_boat.Translate(new_boat_coords);

		Connect("AddedNewWaypoint", new Callable(new_boat, nameof(Boat.add_waypoint)));
		Connect("SelectedBoats", new Callable(new_boat, nameof(Boat.on_selection)));
		Connect("SelectionCleared", new Callable(new_boat, nameof(Boat.on_selection_cleared)));

		GetParent().AddChild(new_boat);
		boats.Add(new_boat);
	}



	private void move_boats(Vector3 pos)
	{

		Hex waypoint = HexMap.get_hex_at_world_coords(pos.X, pos.Z);

		EmitSignal(SignalName.AddedNewWaypoint, waypoint);
	}



	public void select_boats(Aabb selection_box)
	{
		selected_boats.Clear();
		EmitSignal(SignalName.SelectionCleared);

		foreach (Boat boat in boats)
		{
			if (selection_box.HasPoint(boat.Position))
			{
				selected_boats.Add(boat);
				boat.Call(nameof(Boat.on_selection));
			}
		}

		// EmitSignal(SignalName.SelectedBoats, selected_boats.ToArray());
	}


	private void handle_input(float delta)
	{
		Vector3 cameraDirection = -GetViewport().GetCamera3D().GlobalBasis.Z;
		cameraDirection.Y = 0;
		cameraDirection = cameraDirection.Normalized();
		
		if (Input.IsPhysicalKeyPressed(Key.W))
			Position += movement_speed * delta * cameraDirection;
		
		if (Input.IsPhysicalKeyPressed(Key.S))
			Position -= movement_speed * delta * cameraDirection;
		
		if (Input.IsPhysicalKeyPressed(Key.A))
			Position += movement_speed * delta * cameraDirection.Rotated(Vector3.Up, Mathf.Pi / 2.0f);
		
		if (Input.IsPhysicalKeyPressed(Key.D))
			Position -= movement_speed * delta * cameraDirection.Rotated(Vector3.Up, Mathf.Pi / 2.0f);
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

			move_boats(intersection);
		}

		else if (@event is InputEventKey eventKey)
		{
			if (!eventKey.Echo
				&& eventKey.Keycode == Key.N
				&& eventKey.IsPressed())
				add_new_boat();
		}
    }
}
