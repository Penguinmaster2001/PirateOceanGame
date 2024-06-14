using System.Linq;
using Godot;

// This needs to support rectangles in screen space <=> arbitrary convex quadrilaterals in world space
// Maybe need a new class to replace AABB

public partial class SelectionBox : Control
{
    [Signal]
    public delegate void SelectionAreaEventHandler(SelectionPolygon selection_box);


    private Camera3D camera;
    private NinePatchRect box;
    private Vector2 start_mouse_pos;
    private Vector2 end_mouse_pos;

    // 0 - top left, 1 - bottom left, 2 - bottom right, 3 - top right
    private Vector2[] screen_corners = new Vector2[4];

    private Vector3 selection_start;
    private Vector3 selection_end;

    private bool is_selecting = false;

    public override void _Ready()
    {
        camera = GetViewport().GetCamera3D();
        box = GetNode<NinePatchRect>("SelectionRect");
        box.Visible = false;

        Connect("SelectionArea", new Callable(GetNode("/root/Main/FleetController"), nameof(FleetController.select_boats)));

        for (int i = 0; i < 4; i++)
            screen_corners[i] = Vector2.Zero;
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
        // Left - right
        float left_x = start_mouse_pos.X;
        float right_x = end_mouse_pos.X;
        if (left_x > right_x)
        {
            left_x = end_mouse_pos.X;
            right_x = start_mouse_pos.X;
        }

        screen_corners[0].X = left_x;
        screen_corners[1].X = left_x;

        screen_corners[2].X = right_x;
        screen_corners[3].X = right_x;

        // Top - bottom
        float top_y = start_mouse_pos.Y;
        float bottom_y = end_mouse_pos.Y;
        if (top_y > bottom_y)
        {
            top_y = end_mouse_pos.Y;
            bottom_y = start_mouse_pos.Y;
        }

        screen_corners[0].Y = top_y;
        screen_corners[3].Y = top_y;

        screen_corners[1].Y = bottom_y;
        screen_corners[2].Y = bottom_y;


        Vector2 size = new(right_x - left_x, bottom_y - top_y);
        
        box.Position = new(left_x, top_y);
        box.Size = size;
	}

    private void SelectObjects()
    {
        Vector3[] selection_corners = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            Vector3 origin = camera.ProjectRayOrigin(screen_corners[i]);
            Vector3 direction = camera.ProjectRayNormal(screen_corners[i]);

            Vector3 intersection = origin - (direction * origin.Y / direction.Y);
            selection_corners[i] = intersection;
        }

        SelectionPolygon selection_quad = new(selection_corners.ToList());

        EmitSignal("SelectionArea", selection_quad);
    }
}