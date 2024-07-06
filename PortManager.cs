using Godot;
using HexModule;
using System.Collections.Generic;



/*
 * Manages a collection of all the ports
 * Manages displaying the port UI
 * Interacts with a FleetController for boat spawning and boat actions
 */
public partial class PortManager : Control
{
    private readonly Dictionary<Hex, Port> ports = new();

    private Control game_ui;
	private PackedScene port_ui_scene = GD.Load<PackedScene>("res://PortUI.tscn");
	private Control selectedPortUI = null;

    private Port selectedPort = null;


    [Signal]
	public delegate void SpawnNewBoatEventHandler(Vector3 coords);



    public override void _Ready()
    {
        game_ui = GetNode<Control>("/root/Main/GameUI");

		// Connect(SignalName.SpawnNewBoat,
		// 	new Callable(GetNode("/root/Main/FleetController"),
		// 	nameof(FleetController.SpawnNewBoat)));
    }



    public void HandleHexSelection(Hex hex)
    {
		if(hex == null || !hex.TerrainType.Name.Contains("port")) // There needs to be a better way to do this
		{
            ClearSelectedPort();
            return;
		}

        if (!ports.ContainsKey(hex))
            RegisterNewPort(hex);

        if (ports[hex] == selectedPort) return;

        ClearSelectedPort();
        SetSelectedPort(ports[hex]);
    }



    private void SetSelectedPort(Port port)
    {
        selectedPort = port;

        selectedPortUI = port_ui_scene.Instantiate<Control>();

        AddChild(selectedPortUI);

        Button btnNewBoat = (Button) selectedPortUI.FindChild("NewBoatButton");
        btnNewBoat.Pressed += HandleNewBoatButtonPress;

        Label lblPortName = (Label) selectedPortUI.FindChild("PortName");
        lblPortName.Text = selectedPort.ToString();
    }



    private void ClearSelectedPort()
    {
        selectedPort = null;

        if (selectedPortUI != null && selectedPortUI.IsInsideTree())
        {
            selectedPortUI.QueueFree();

            // We will treat the reference as invalid as this point
            selectedPortUI = null;
        }
    }



    private void RegisterNewPort(Hex hex)
    {
        ports.Add(hex, new(hex));
    }



    public void HandleNewBoatButtonPress()
    {
        EmitSignal(SignalName.SpawnNewBoat, selectedPort.NewBoatSpawnLocation);
    }
}
