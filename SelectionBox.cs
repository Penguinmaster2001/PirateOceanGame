using Godot;

// This needs to support rectangles in screen space <=> arbitrary convex quadrilaterals in world space
// Maybe need a new class to replace AABB

public partial class SelectionBox : Node3D
{
    [Signal]
    public delegate void SelectionAreaEventHandler(Aabb selection_box);


    private Camera3D camera;
    private MeshInstance3D box;
    private Vector2 start_mouse_pos;
    private Vector2 end_mouse_pos;

    private Vector3 selection_start;
    private Vector3 selection_end;

    private bool is_selecting = false;

    public override void _Ready()
    {
        camera = GetViewport().GetCamera3D();
        box = GetNode<MeshInstance3D>("Box");
        box.Visible = false;

        Connect("SelectionArea", new Callable(GetNode("../FleetController"), nameof(FleetController.select_boats)));
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            start_mouse_pos = GetViewport().GetMousePosition();
            is_selecting = true;
            box.Visible = true;
        }

        if (is_selecting)
        {
            end_mouse_pos = GetViewport().GetMousePosition();
            UpdateSelectionBox();
        }

        if (Input.IsActionJustReleased("ui_select"))
        {
            SelectObjects();
            is_selecting = false;
            box.Visible = false;
        }
    }

	private void UpdateSelectionBox()
	{
        Vector3 start_origin = camera.ProjectRayOrigin(start_mouse_pos);
        Vector3 start_direction = camera.ProjectRayNormal(start_mouse_pos);

        selection_start = start_origin - (start_direction * start_origin.Y / start_direction.Y);

        Vector3 end_origin = camera.ProjectRayOrigin(end_mouse_pos);
        Vector3 end_direction = camera.ProjectRayNormal(end_mouse_pos);

        selection_end = start_origin - (end_direction * end_origin.Y / end_direction.Y);
        
		Vector3 size = selection_end - selection_start;
		Vector3 center = 0.5f * (selection_start + selection_end);
        
        box.Position = center;
        box.Scale = size + new Vector3(0, 5, 0);
	}

    private void SelectObjects()
    {
        Aabb selection_box = new Aabb(selection_start, selection_end - selection_start + new Vector3(0.0f, 10.0f, 0.0f)).Abs();

        EmitSignal(SignalName.SelectionArea, selection_box);
    }
}