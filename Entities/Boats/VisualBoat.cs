
using Godot;

using System;
using System.Collections.Generic;

using Entities.Boats.BoatParts;



namespace Entities.Boats;



/// <summary>
/// For controlling the visual position of a boat and managing the visual parts.
/// It has an EventHandler TargetReached that is invoked each time it reaches the target Hex.
/// </summary>
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



    public void UpdateBoatParts(List<BoatPart> boatParts)
    {
        // Remove all previous boat parts in the scene
        foreach (Node oldBoatPart in GetChildren())
        {
            oldBoatPart.QueueFree();
        }

        // Add new boat parts to the scene
        foreach (BoatPart boatPart in boatParts)
        {
            AddChild(boatPart.EntityNode);
        }
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
