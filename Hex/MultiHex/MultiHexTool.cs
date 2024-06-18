
using System.Collections.Generic;
using Godot;

public partial class MultiHexTool : Control
{
	private PackedScene main_menu = GD.Load<PackedScene>("res://StartMenu.tscn");

	private float size_ratio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hex_tile = GD.Load<PackedScene>("res://Hex/HexTile.tscn");
    
	private int grid_size = 5;


	private List<Material> hex_materials = new();

	private Dictionary<Hex, Node3D> displayed_hexes = new();

	private ItemList hex_type_options;

	private WfcHex selected_hex;

	private Label3D[] edge_hints = new Label3D[6];

	private MultiHex multi_hex;


	public override void _Ready()
	{
		List<string> material_paths = HexTypes.get_type_material_paths();

		foreach (string path in material_paths)
			hex_materials.Add(GD.Load<Material>(path));

		multi_hex = new(HexContainer.MapShape.hexagon, grid_size);


		foreach (Hex hex in multi_hex.get_uncollapsed_hexes())
			display_hex(hex);

		hex_type_options = (ItemList) FindChild("HexTypeOptions");

		hex_type_options.ItemSelected += on_item_selection;
		hex_type_options.ItemActivated += on_item_activation;


		for (int i = 0; i < 6; i++)
		{
			edge_hints[i] = GD.Load<PackedScene>("res://Hex/MultiHex/TypeHint.tscn").Instantiate<Label3D>();

			AddChild(edge_hints[i]);
		}
	}



    public override void _Process(double delta)
    {
        if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().ChangeSceneToPacked(main_menu);

			multi_hex.clear();
		}
    }



	private void update_multihex()
	{
		foreach (Hex hex in multi_hex.get_collapsed_hexes())
		{
			remove_hex(hex);

			display_hex(hex);
		}
	}
		


	private void display_hex(Hex hex)
	{
		if (hex == null) return;

		Vector3 hex_coords = hex.get_world_coords();

		Node3D display_hex = hex_tile.Instantiate<Node3D>();

		AddChild(display_hex);
		displayed_hexes.Add(hex, display_hex);

		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = display_hex.GetChild<MeshInstance3D>(0);
		hex_mesh.MaterialOverride = hex_materials[hex.get_terrain_type()];
	}



	private void remove_hex(Hex hex)
	{
		if (displayed_hexes.ContainsKey(hex))
		{
			Node3D tile_to_remove;

			displayed_hexes.Remove(hex, out tile_to_remove);

			tile_to_remove.QueueFree();
		}
	}



	private void on_hex_selection(Hex hex)
	{
		if (hex is WfcHex selected_hex)
		{
			this.selected_hex = selected_hex;

			hex_type_options.Clear();

			foreach (int type in selected_hex.get_allowed_types())
			{
				hex_type_options.AddItem(HexTypes.get_name(type));
			}


			List<int> types = selected_hex.get_edge_types();
			for (int i = 0; i < 6; i++)
			{
				edge_hints[i].Position = selected_hex.get_world_coords() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
				edge_hints[i].Text = types[i].ToString();
			}
		}
	}



	private void on_item_selection(long index)
	{
		int[] types = HexTypes.get_type_edges(selected_hex.get_allowed_types()[(int) index]);
		for (int i = 0; i < 6; i++)
		{
			edge_hints[i].Position = selected_hex.get_world_coords() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
			edge_hints[i].Text = types[i].ToString();
		}
	}



	private void on_item_activation(long index)
	{
		multi_hex.collapse_hex(selected_hex, selected_hex.get_allowed_types()[(int) index]);
		update_multihex();
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

			Hex selected_hex = multi_hex.get_hex_at_world_coords(intersection.X, intersection.Z);
			on_hex_selection(selected_hex);
		}
    }
}
