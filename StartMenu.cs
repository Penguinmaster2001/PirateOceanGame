using Godot;

public partial class StartMenu : Control
{
    private Button start_game_button;
    private PackedScene main_game = GD.Load<PackedScene>("res://main.tscn");

    private Button hex_tool_button;
    private PackedScene hex_tool = GD.Load<PackedScene>("res://Hex/MultiHex/MultiHexTool.tscn");

    public override void _Ready()
    {
        start_game_button = (Button) FindChild("StartGameButton");
        start_game_button.Pressed += start_game;

        hex_tool_button = (Button) FindChild("StartMultiHexToolButton");
        hex_tool_button.Pressed += open_hex_tool;
    }



    private void start_game()
    {
        GetTree().ChangeSceneToPacked(main_game).ToString();
    }



    private void open_hex_tool()
    {
        GetTree().ChangeSceneToPacked(hex_tool);
    }
}
