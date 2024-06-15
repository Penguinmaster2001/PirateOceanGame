using Godot;
using System;
using System.Collections.Generic;


/*
 * Simple class for selecting stuff
 * The main use is checking if a boat is inside the selection box on the screen
 * But in world coordinates, the selection box is not rectangular
 * This class only works on a plane, the Z coord becomes the Y coord
 */
public partial class SelectionPolygon : GodotObject
{
    private List<Vector2> corners = new();

    private Vector2 point_outside = Vector2.Zero;

    public SelectionPolygon(List<Vector3> corners)
    {
        // Find a point outside
        foreach (Vector3 corner in corners)
        {
            this.corners.Add(new(corner.X, corner.Z));

            if (point_outside.X < corner.X)
                point_outside.X = corner.X + 1000.0f;

            if (point_outside.Y < corner.Z)
                point_outside.Y = corner.Z + 1000.0f;
        }
    }



    private bool intersect_line(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        // Line segment intersection algorithm

        float denominator = ((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X));

        // Parallel
        if (!(denominator < -float.Epsilon || float.Epsilon < denominator))
            return false;

        float t = (((a1.X - b1.X) * (b1.Y - b2.Y)) - ((a1.Y - b1.Y) * (b1.X - b2.X))) / denominator;

        if (t < 0.0f || 1.0 < t)
            return false;

        float u = -(((a1.X - a2.X) * (a1.Y - b1.Y)) - ((a1.Y - a2.Y) * (a1.X - b1.X))) / denominator;

        if (u < 0.0f || 1.0 < u)
            return false;

        return true;
    }



    public bool has_point(Vector3 point)
    {
        bool inside = false;
        Vector2 test_point = new(point.X, point.Z);

        for (int i = 0; i < corners.Count; i++)
        {
            if (intersect_line(test_point, point_outside, corners[i], corners[(i + 1) % corners.Count]))
                inside = !inside;
        }

        return inside;
    }



    public override string ToString()
    {
        string to_string = "Selection polygon:\n";

        for (int i = 0; i < corners.Count; i++)
            to_string += i + ": " + corners[i].ToString() + "\n";

        to_string += "Point outside: " + point_outside.ToString() + "\n";

        return to_string;
    }
}
