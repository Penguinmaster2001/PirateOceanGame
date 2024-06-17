using Godot;
using System.Collections.Generic;

public partial class HexDisplay : Node3D
{
	private float size_ratio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hex_tile = GD.Load<PackedScene>("res://Hex/HexTile.tscn");
    
	private int grid_size = 5;


	private List<Material> hex_materials = new();



	[Signal]
	public delegate void HexSelectedEventHandler(Hex hex);



	public override void _Ready()
	{
		List<string> material_paths = HexTypes.get_type_material_paths();

		foreach (string path in material_paths)
			hex_materials.Add(GD.Load<Material>(path));

		HexMap.generate_hexagon(grid_size);

		// Connect(SignalName.HexSelected,
		// 	new Callable(GetNode("/root/MultiHexTool"),
		// 	nameof(MultiHexTool.on_hex_selection)));
	}



	public override void _Process(double delta)
	{
		if (HexMap.collapsed_all_hexes()) return;

		ulong start_ms = Time.GetTicksMsec();
		while (Time.GetTicksMsec() - start_ms < 10)
			display_hex(HexMap.collapse_next_hex());
	}



	private void display_map(List<WfcHex> hexes)
	{
		foreach (Hex hex in hexes)
			display_hex(hex);
	}
		


	private void display_hex(Hex hex)
	{
		if (hex == null) return;

		Vector3 hex_coords = hex.get_world_coords();

		Node3D display_hex = hex_tile.Instantiate<Node3D>();

		AddChild(display_hex);
		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = display_hex.GetChild<MeshInstance3D>(0);
		hex_mesh.MaterialOverride = hex_materials[hex.get_terrain_type()];
	}



	private void select_hex(Hex hex)
	{
        GD.Print("Selection");

		EmitSignal(SignalName.HexSelected, hex);
	}
	


    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseButton eventMouseButton
			&& eventMouseButton.IsPressed()
			&& eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			Camera3D camera = GetViewport().GetCamera3D();

			Vector3 origin = camera.ProjectRayOrigin(eventMouseButton.Position);
			Vector3 direction = camera.ProjectRayNormal(eventMouseButton.Position);

			Vector3 intersection = origin - (direction * origin.Y / direction.Y);

			Hex selected_hex = HexMap.get_hex_at_world_coords(intersection.X, intersection.Z);
			select_hex(selected_hex);
		}
    }



}
