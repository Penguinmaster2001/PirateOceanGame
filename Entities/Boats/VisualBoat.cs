
using System;
using Godot;



namespace Entities.Boats;



public partial class VisualBoat : Node3D
{
    public Vector3 TargetPos { get; set; } = new();
    public float BoatSpeed { get; set; } = 100.0f;

    public bool Moving { get; set; } = false;

    public event EventHandler TargetReached;



    public override void _Ready()
    {
        TargetPos = Position;
    }



    public override void _Process(double delta)
    {
        NavigateToTarget((float) delta);
    }



    private void NavigateToTarget(float delta)
    {
        float distanceToTarget = Position.DistanceTo(TargetPos);
        Vector3 directionToTarget = Position.DirectionTo(TargetPos);

        if (distanceToTarget > 5.0f)
            Position += directionToTarget * BoatSpeed * Mathf.Clamp(distanceToTarget / 10.0f, 0.25f, 3.0f) * delta;

        if (!Moving) return;

        if (distanceToTarget < 35.0f)
        {
            TargetReached?.Invoke(this, EventArgs.Empty);
        }
    }
}
