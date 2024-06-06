using Godot;
using System;
using System.Collections.Generic;

public partial class HexGrid : Node3D
{
	private float size_ratio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hex_tile = (PackedScene) GD.Load("res://hex_tile.tscn");

	[Export(PropertyHint.Range, "1, 500")]
	private int grid_size;


	private List<Material> hex_materials = new();



	public override void _Ready()
	{
		List<String> material_paths = HexTypes.get_type_material_paths();

		foreach (String path in material_paths)
		{
			hex_materials.Add((Material) GD.Load(path));
		}

		HexMap.generate_triangle(grid_size);

		display_map(HexMap.get_collapsed_hexes());
	}



	public override void _Process(double delta)
	{
		if (HexMap.collapsed_all_hexes())
			return;

		// for (int i = 0; i < 60; i++)
		// 	display_hex(HexMap.collapse_next_hex());

		ulong start_ms = Time.GetTicksMsec();
		while (Time.GetTicksMsec() - start_ms < 10)
			display_hex(HexMap.collapse_next_hex());

		// HexMap.display_data()
	}



	private void display_map(List<WfcHex> hexes)
	{
		foreach (Hex hex in hexes)
			display_hex(hex);
	}
		


	private void display_hex(Hex hex)
	{
		if (hex == null)
			return;

		Vector3 hex_coords = hex.get_world_coords();

		Node3D display_hex = (Node3D) hex_tile.Instantiate();
		AddChild(display_hex);
		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = (MeshInstance3D) display_hex.GetChild(0);
		hex_mesh.MaterialOverride = hex_materials[hex.get_terrain_type()];
	}
}
