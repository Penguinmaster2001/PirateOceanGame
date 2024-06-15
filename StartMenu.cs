using Godot;
using System;

public partial class StartMenu : Control
{
    private Button start_game_button;

    private PackedScene main_game = GD.Load<PackedScene>("res://main.tscn");

    public override void _Ready()
    {
        start_game_button = (Button) FindChild("StartGameButton");

        start_game_button.Pressed += start_game;
    }



    private void start_game()
    {
        GetTree().ChangeSceneToPacked(main_game);
    }
}
