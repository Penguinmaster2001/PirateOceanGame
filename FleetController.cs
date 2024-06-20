using Godot;
using Godot.Collections;
using HexModule;
using System.Collections.Generic;

public partial class FleetController : Node3D
{
	[Signal]
	public delegate void AddedNewWaypointEventHandler(Vector3 waypoint);

	[Signal]
	public delegate void SelectedBoatsEventHandler(Array<Node3D> boats_selected);

	[Signal]
	public delegate void SelectionClearedEventHandler();


	private PackedScene boatScene = GD.Load<PackedScene>("res://Boat/boat.tscn");

	private readonly List<Boat> boats = new();
	private readonly List<Boat> selectedBoats = new();

	private Label lblBoatInfo;

	private Camera3D mainCamera;

	private float movementSpeed = 1000.0f;

	private HexMap hexMap;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hexMap = GetNode<HexGrid>("/root/Main/HexGrid").hexMap;

		lblBoatInfo = (Label) GetNode("/root/Main/GameUI").FindChild("BoatInfo");

		mainCamera = GetViewport().GetCamera3D();
	}



	public override void _Process(double delta)
	{
		HandleInput((float) delta);
	}



	public void SpawnNewBoat(Vector3 boatSpawnCoordinates)
	{
		boatSpawnCoordinates.Y = 0;

		Boat spawnedBoat = boatScene.Instantiate<Boat>();
		spawnedBoat.Translate(boatSpawnCoordinates);

		spawnedBoat.hex_map = hexMap;

		Connect(SignalName.AddedNewWaypoint, new Callable(spawnedBoat, nameof(Boat.add_waypoint)));
		Connect(SignalName.SelectedBoats, new Callable(spawnedBoat, nameof(Boat.on_selection)));
		Connect(SignalName.SelectionCleared, new Callable(spawnedBoat, nameof(Boat.on_selection_cleared)));

		GetParent().AddChild(spawnedBoat);
		boats.Add(spawnedBoat);
	}



	public void HandleHexSelection(Hex hex)
	{
		if (hex == null) return;

		if (selectedBoats.Count > 0 && hex.TerrainType.Traversable)
			AddNewBoatWaypoint(hex.WorldCoordinates());
	}



	private void AddNewBoatWaypoint(Vector3 pos)
	{
		Hex waypoint = hexMap.HexAtWorldCoordinates(pos.X, pos.Z);

		EmitSignal(SignalName.AddedNewWaypoint, waypoint);
	}



	public void UpdateSelectedBoats(SelectionPolygon selection_quad)
	{
		selectedBoats.Clear();
		EmitSignal(SignalName.SelectionCleared);

		foreach (Boat boat in boats)
		{
			if (selection_quad.has_point(boat.Position))
			{
				selectedBoats.Add(boat);
				boat.Call(nameof(Boat.on_selection));
			}
		}

		if (selectedBoats.Count <= 0)
			lblBoatInfo.Text = "No boats selected";
		else if (selectedBoats.Count == 1)
			lblBoatInfo.Text = selectedBoats[0].ToString();
		else
			lblBoatInfo.Text = selectedBoats.Count + " Boats Selected";
	}



	private void HandleInput(float delta)
	{
		Vector3 cameraDirection = -mainCamera.GlobalBasis.Z;
		cameraDirection.Y = 0;
		cameraDirection = cameraDirection.Normalized();

		float speed = movementSpeed * delta * Mathf.Clamp(mainCamera.Position.Y / 1000.0f, 0.25f, 3.0f);
		
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
