
using Godot;

using System;
using System.Linq;
using System.Collections.Generic;

using HexModule;
using HexModule.Map;



namespace Entities.Boats;



public class Boat : RegisterableEntity, IVisual3D, ICommandable //, ISelectable?
{
    private static readonly NavigableTag[] BoatNavigableTags = { new(0) };

    public bool Moving { get; private set; }

    public float MovementSpeed { get; private set; }

    private readonly Queue<Hex> Waypoints;

    private readonly Navigator MapNavigator;

    public Hex MapPosition { get; private set; }

    public Node3D EntityNode { get; private set; }
    private readonly VisualBoat visualBoat;

    // Boat parts



    public Boat(HexMap map, Hex mapPosition)
    {
        Moving = false;

        // TODO: Calculate speed from boat parts
        MovementSpeed = 100.0f;

        Waypoints = new();

        MapNavigator = new(map, BoatNavigableTags.ToHashSet());

        MapPosition = mapPosition;

        EntityNode = GD.Load<Node3D>("res://Boat/boat.tscn");

        visualBoat = (VisualBoat) EntityNode;

        visualBoat.TargetReached += OnTargetReached;

        Name = "Ship No. " + ID;
    }



    public void AddWaypoint(Hex waypoint)
    {
        MapNavigator.TryNavigateStraight(Waypoints.Last(), waypoint, Waypoints);
    }



    public void StartMoving()
    {
        Moving = true;
        MoveToNextTarget();
    }



    public void StopMoving()
    {
        Moving = false;
        visualBoat.Moving = false;
    }



    public void AddToScene(Node parentNode)
    {
        parentNode.AddChild(visualBoat);
    }



    public void RemoveFromScene()
    {
        visualBoat.QueueFree();
    }



    private void OnTargetReached(object sender, EventArgs e)
    {
        MoveToNextTarget();
    }



    private void MoveToNextTarget()
    {
        if (!Moving) return;

        if (Waypoints.TryDequeue(out Hex nextWaypoint))
        {
            UpdateVisualPosition(nextWaypoint);
        }
        else
        {
            Moving = false;

            visualBoat.Moving = false;
            // Maybe need to invoke something for the IController
        }
    }



    public void UpdateVisualPosition(Hex newPosition)
    {
        visualBoat.Moving = true;

        visualBoat.TargetPos = newPosition.WorldCoordinates();
    }
}
