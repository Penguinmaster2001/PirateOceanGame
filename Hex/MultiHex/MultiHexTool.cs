
using System.Collections.Generic;
using System.Linq;
using Godot;
using HexModule;

public partial class MultiHexTool : Control
{
	private PackedScene startMenuScene = GD.Load<PackedScene>("res://StartMenu.tscn");

	private float hexSideRatio = Mathf.Cos(Mathf.Pi / 6.0f);
	private PackedScene hex_tile = GD.Load<PackedScene>("res://Hex/HexTile.tscn");
    
	private int grid_size = 5;
	

	private readonly Dictionary<Hex, Node3D> visibleHexTiles = new();

	private ItemList hexTypeDisplay;

	private WfcHex selectedHex;

	private Label3D[] hexEdgeLabels = new Label3D[6];

	private HexMap hexMap;


	public override void _Ready()
	{
		hexMap = new(HexContainer.MapShape.hexagon, grid_size);


		foreach (Hex hex in hexMap.UncollapsedHexes)
			ShowHex(hex);

		hexTypeDisplay = (ItemList) FindChild("HexTypeOptions");

		hexTypeDisplay.ItemSelected += HandleItemSelection;
		hexTypeDisplay.ItemActivated += HandleItemActivation;


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

			hexMap.Clear();
		}
    }



	private void UpdateMultiHex()
	{
		foreach (Hex hex in hexMap.CollapsedHexes)
		{
			RemoveHex(hex);

			ShowHex(hex);
		}
	}
		


	private void ShowHex(Hex hex)
	{
		if (hex == null) return;

		Vector3 hex_coords = hex.WorldCoordinates();

		Node3D display_hex = hex_tile.Instantiate<Node3D>();

		AddChild(display_hex);
		visibleHexTiles.Add(hex, display_hex);

		display_hex.Translate(hex_coords);
		
		MeshInstance3D hex_mesh = display_hex.GetChild<MeshInstance3D>(0);
		hex_mesh.MaterialOverride = GD.Load<Material>(hex.TerrainType.MaterialPath);
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
		if (hex is WfcHex newSelectedHex)
		{
			selectedHex = newSelectedHex;

			hexTypeDisplay.Clear();

			foreach (HexType type in newSelectedHex.ValidHexTypes)
			{
				hexTypeDisplay.AddItem(type.Name);
			}


			EdgeType[] edgeTypes = newSelectedHex.TerrainType.EdgeTypes;
			for (int i = 0; i < 6; i++)
			{
				hexEdgeLabels[i].Position = newSelectedHex.WorldCoordinates() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
				hexEdgeLabels[i].Text = edgeTypes[i].ToString();
			}
		}
	}



	private void HandleItemSelection(long index)
	{
		EdgeType[] edgeTypes = selectedHex.ValidHexTypes.ToArray()[(int) index].EdgeTypes;
		for (int i = 0; i < 6; i++)
		{
			hexEdgeLabels[i].Position = selectedHex.WorldCoordinates() + (25.0f * new Vector3(Mathf.Cos(i * Mathf.Pi / 3.0f), 1.0f, Mathf.Sin(i * Mathf.Pi / 3.0f)));
			hexEdgeLabels[i].Text = edgeTypes[i].ToString();
		}
	}



	private void HandleItemActivation(long index)
	{
		hexMap.CollapseHex(selectedHex, selectedHex.ValidHexTypes.ToArray()[(int) index]);
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

			Hex selected_hex = hexMap.HexAtWorldCoordinates(intersection.X, intersection.Z);
			HandleHexSelection(selected_hex);
		}
    }
}
