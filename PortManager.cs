using Godot;
using System.Collections.Generic;



/*
 * Manages a collection of all the ports
 * Manages displaying the port UI
 * Interacts with a FleetController for boat spawning and boat actions
 */
public partial class PortManager : Control
{
    private Dictionary<Hex, Port> ports = new();

    private Control game_ui;
	private PackedScene port_ui_scene = GD.Load<PackedScene>("res://PortUI.tscn");
	private Control current_port_ui = null;

    private Port selected_port = null;


    [Signal]
	public delegate void SpawnNewBoatEventHandler(Vector3 coords);



    public override void _Ready()
    {
        game_ui = GetNode<Control>("/root/Main/GameUI");

		Connect(SignalName.SpawnNewBoat,
			new Callable(GetNode("/root/Main/FleetController"),
			nameof(FleetController.add_new_boat)));
    }



    public void on_hex_selection(Hex hex)
    {
		if(hex == null || !HexTypes.get_name(hex.getTerrainType()).Contains("port")) // There needs to be a better way to do this
		{
            deselect_port();

            return;
		}

        if (!ports.ContainsKey(hex))
            add_new_port(hex);

        if (ports[hex] == selected_port) return;

        deselect_port();
        select_port(ports[hex]);
    }



    private void select_port(Port port)
    {
        selected_port = port;

        current_port_ui = port_ui_scene.Instantiate<Control>();

        AddChild(current_port_ui);

        Button new_boat_button = (Button) current_port_ui.FindChild("NewBoatButton");
        new_boat_button.Pressed += on_new_boat_button_pressed;

        Label port_name_label = (Label) current_port_ui.FindChild("PortName");
        port_name_label.Text = selected_port.ToString();
    }



    private void deselect_port()
    {
        selected_port = null;

        if (current_port_ui != null && current_port_ui.IsInsideTree())
        {
            current_port_ui.QueueFree();

            // We will treat the reference as invalid as this point
            current_port_ui = null;
        }
    }



    private void add_new_port(Hex hex)
    {
        ports.Add(hex, new(hex));
    }



    public void on_new_boat_button_pressed()
    {
        EmitSignal(SignalName.SpawnNewBoat, selected_port.get_spawn_location());
    }
}
