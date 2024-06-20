
using Godot;

public partial class Main : Node3D
{
	private PackedScene main_menu = GD.Load<PackedScene>("res://StartMenu.tscn");



	public override void _Process(double delta)
	{
        if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().ChangeSceneToPacked(main_menu);
		}
	}
}
