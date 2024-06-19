
using System.Collections.Generic;
using Godot;
using HexModule;

public partial class MultiHexTool : Control
{
	private PackedScene startMenuScene = GD.Load<PackedScene>("res://StartMenu.tscn");

	private float hexSideRatio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hex_tile = GD.Load<PackedScene>("res://Hex/HexTile.tscn");
    
	private int grid_size = 5;
	

	private Dictionary<Hex, Node3D> visibleHexTiles = new();

	private ItemList hexTypeDisplay;

	private WfcHex selectedHex;

	private Label3D[] hexEdgeLabels = new Label3D[6];

	private MultiHex multiHex;


	public override void _Ready()
	{
		multiHex = new(HexContainer.MapShape.hexagon, grid_size);


		foreach (Hex hex in multiHex.UncollapsedHexes)
			display_hex(hex);

		hexTypeDisplay = (ItemList) FindChild("HexTypeOptions");

		hexTypeDisplay.ItemSelected += HandleItemSelection;
		hexTypeDisplay.ItemActivated += on_item_activation;


		for (int i = 0; i < 6; i++)
		{
			hexEdgeLabels[i] = GD.Load<PackedScene>("res://Hex/MultiHex/TypeHint.tscn").Instantiate<Label3D>();

			AddChild(hexEdgeLabels[i]);
		}
	}



    public override void _Process(double delta)
    {
        if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().ChangeSceneToPacked(startMenuScene);

			multiHex.clear();
		}
    }



	private void UpdateMultiHex()
	{
		foreach (Hex hex in multiHex.CollapsedHexes)
		{
			RemoveHex(hex);

			display_hex(hex);
		}
	}
		


	private void display_hex(Hex hex)
	{
		if (hex == null) return;

		Vector3 hex_coords = hex.GetWorldCoordinates();

		Node3D display_hex = hex_tile.Instantiate<Node3D>();

		AddChild(display_hex);
		visibleHexTiles.Add(hex, display_hex);

		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = display_hex.GetChild<MeshInstance3D>(0);
		hex_mesh.MaterialOverride = hex.TerrainType.Material;
	}



	private void RemoveHex(Hex hex)
	{
		if (visibleHexTiles.ContainsKey(hex))
		{
            visibleHexTiles.Remove(hex, out Node3D tileToRemove);

            tileToRemove.QueueFree();
		}
	}



	private void HandleHexSelection(Hex hex)
	{
		if (hex is WfcHex selected_hex)
		{
			this.selectedHex = selected_hex;

			hexTypeDisplay.Clear();

			foreach (HexType type in selected_hex.ValidHexTypes)
			{
				hexTypeDisplay.AddItem(type.Name);
			}


			EdgeType[] edgeTypes = selected_hex.TerrainType.Edges;
			for (int i = 0; i < 6; i++)
			{
				hexEdgeLabels[i].Position = selected_hex.GetWorldCoordinates() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
				hexEdgeLabels[i].Text = edgeTypes[i].ToString();
			}
		}
	}



	private void HandleItemSelection(long index)
	{
		EdgeType[] edgeTypes = selectedHex.ValidHexTypes[(int) index].Edges;
		for (int i = 0; i < 6; i++)
		{
			hexEdgeLabels[i].Position = selectedHex.GetWorldCoordinates() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
			hexEdgeLabels[i].Text = edgeTypes[i].ToString();
		}
	}



	private void on_item_activation(long index)
	{
		multiHex.collapse_hex(selectedHex, selectedHex.ValidHexTypes[(int) index]);
		UpdateMultiHex();
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

			Hex selected_hex = multiHex.HexAtWorldCoordinates(intersection.X, intersection.Z);
			HandleHexSelection(selected_hex);
		}
    }
}
