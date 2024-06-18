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


	private PackedScene boat_scene = GD.Load<PackedScene>("res://Boat/boat.tscn");

	private List<Boat> boats = new();
	private List<Boat> selected_boats = new();

	private Label boat_info;

	private Camera3D main_camera;

	private float movement_speed = 1000.0f;

	private HexMap hex_map;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hex_map = GetNode<HexGrid>("/root/Main/HexGrid").hex_map;

		boat_info = (Label) GetNode("/root/Main/GameUI").FindChild("BoatInfo");

		main_camera = GetViewport().GetCamera3D();
	}



	public override void _Process(double delta)
	{
		handle_input((float) delta);
	}



	public void add_new_boat(Vector3 new_boat_coords)
	{
		new_boat_coords.Y = 0;

		Boat new_boat = boat_scene.Instantiate<Boat>();
		new_boat.Translate(new_boat_coords);

		new_boat.hex_map = hex_map;

		Connect(SignalName.AddedNewWaypoint, new Callable(new_boat, nameof(Boat.add_waypoint)));
		Connect(SignalName.SelectedBoats, new Callable(new_boat, nameof(Boat.on_selection)));
		Connect(SignalName.SelectionCleared, new Callable(new_boat, nameof(Boat.on_selection_cleared)));

		GetParent().AddChild(new_boat);
		boats.Add(new_boat);
	}



	public void on_hex_selection(Hex hex)
	{
		if (hex == null)
			return;

		if (selected_boats.Count > 0 && HexTypes.is_type_traversable(hex.get_terrain_type()))
			move_boats(hex.get_world_coords());
	}



	private void move_boats(Vector3 pos)
	{
		Hex waypoint = hex_map.get_hex_at_world_coords(pos.X, pos.Z);

		EmitSignal(SignalName.AddedNewWaypoint, waypoint);
	}



	public void select_boats(SelectionPolygon selection_quad)
	{
		selected_boats.Clear();
		EmitSignal(SignalName.SelectionCleared);

		foreach (Boat boat in boats)
		{
			if (selection_quad.has_point(boat.Position))
			{
				selected_boats.Add(boat);
				boat.Call(nameof(Boat.on_selection));
			}
		}

		if (selected_boats.Count <= 0)
			boat_info.Text = "No boats selected";
		else if (selected_boats.Count == 1)
			boat_info.Text = selected_boats[0].ToString();
		else
			boat_info.Text = selected_boats.Count + " Boats Selected";
	}


	private void handle_input(float delta)
	{
		Vector3 cameraDirection = -main_camera.GlobalBasis.Z;
		cameraDirection.Y = 0;
		cameraDirection = cameraDirection.Normalized();

		float speed = movement_speed * delta * Mathf.Clamp(main_camera.Position.Y / 1000.0f, 0.25f, 3.0f);
		
		if (Input.IsPhysicalKeyPressed(Key.W))
			Position += speed * cameraDirection;
		
		if (Input.IsPhysicalKeyPressed(Key.S))
			Position -= speed * cameraDirection;
		
		if (Input.IsPhysicalKeyPressed(Key.A))
			Position += speed * cameraDirection.Rotated(Vector3.Up, Mathf.Pi / 2.0f);
		
		if (Input.IsPhysicalKeyPressed(Key.D))
			Position -= speed * cameraDirection.Rotated(Vector3.Up, Mathf.Pi / 2.0f);
	}
}
