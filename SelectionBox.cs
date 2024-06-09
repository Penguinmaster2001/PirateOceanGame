using Godot;

public partial class SelectionBox : Node3D
{
    [Signal]
    public delegate void SelectionAreaEventHandler(Aabb selection_box);


    private Camera3D camera;
    private MeshInstance3D box;
    private Vector2 startMousePos;
    private Vector2 endMousePos;

    private Vector3 selection_start;
    private Vector3 selection_end;

    private bool isSelecting = false;

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
            startMousePos = GetViewport().GetMousePosition();
            isSelecting = true;
            box.Visible = true;
        }

        if (isSelecting)
        {
            endMousePos = GetViewport().GetMousePosition();
            UpdateSelectionBox();
        }

        if (Input.IsActionJustReleased("ui_select"))
        {
            SelectObjects();
            isSelecting = false;
            box.Visible = false;
        }
    }

	private void UpdateSelectionBox()
	{
        Vector3 start_origin = camera.ProjectRayOrigin(startMousePos);
        Vector3 start_direction = camera.ProjectRayNormal(startMousePos);

        selection_start = start_origin - (start_direction * start_origin.Y / start_direction.Y);

        Vector3 end_origin = camera.ProjectRayOrigin(endMousePos);
        Vector3 end_direction = camera.ProjectRayNormal(endMousePos);

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