using Godot;
using System.Collections.Generic;
using HexModule;



/*
 * This manages the HexMap
 * It displays hexes generated by the map
 * Eventually it will hide hexes out of range
 * 
 * It also manages clicking on the hexes
 */
public partial class HexGrid : Node3D
{
	private float hexSideRatio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hexTileScene = GD.Load<PackedScene>("res://Hex/HexTile.tscn");

	[Export(PropertyHint.Range, "1, 500")]
	private int GridSize;

	public HexMap hexMap;



	[Signal]
	public delegate void HexSelectedEventHandler(Hex hex);



	public override void _Ready()
	{
		hexMap = new(HexContainer.MapShape.triangle, GridSize);

		// hexMap.PopulateHexType(20, "super_deep_water");
		// hexMap.PopulateHexType(4, "mountain");

		ShowMap(hexMap.CollapsedHexes);

		Connect(SignalName.HexSelected,
			new Callable(GetNode("/root/Main/FleetController"),
			nameof(FleetController.HandleHexSelection)));

		Connect(SignalName.HexSelected,
			new Callable(GetNode("/root/Main/GameUI/PortManager"),
			nameof(PortManager.HandleHexSelection)));
	}


	double avePerFrame = 0;
	int total = 0;
	int frames = 0;
	public override void _Process(double delta)
	{
		if (hexMap.AllHexesCollapsed()) return;

		int numThisFrame = 0;
		frames++;

		ulong start_ms = Time.GetTicksMsec();
		do
		{
			if (!hexMap.TryCollapseNextHex(out WfcHex nextCollapsedHex)) break;
			
			ShowHex(nextCollapsedHex);
			numThisFrame++;
			total++;
		}
		while (Time.GetTicksMsec() - start_ms < 10);

		avePerFrame = (double) total / (double) frames;

		GD.Print("Total: " + total);
		GD.Print("This frame: " + numThisFrame);
		GD.Print("Average: " + avePerFrame);
	}



	private void ShowMap(List<WfcHex> hexes)
	{
		foreach (Hex hex in hexes)
			ShowHex(hex);
	}
		


	private void ShowHex(Hex hex)
	{
		if (hex == null) return;

		Vector3 hex_coords = hex.WorldCoordinates();

		Node3D display_hex = hexTileScene.Instantiate<Node3D>();

		AddChild(display_hex);
		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = display_hex.GetChild<MeshInstance3D>(0);
		hex_mesh.MaterialOverride = hex.TerrainType.HexMaterial;
	}



	private void SelectHex(Hex hex)
	{
		EmitSignal(SignalName.HexSelected, hex);
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

			Hex selected_hex = hexMap.HexAtWorldCoordinates(intersection.X, intersection.Z);
			SelectHex(selected_hex);
		}
    }
}
