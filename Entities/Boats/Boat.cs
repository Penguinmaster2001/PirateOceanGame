
using Godot;

using System;
using System.Linq;
using System.Collections.Generic;

using Entities.Boats.BoatParts;

using HexModule;
using HexModule.Map;



namespace Entities.Boats;



/// <summary>
/// Manages Boat movement.
/// Makes a Navigator, Assembly, and VisualBoat work together.
/// </summary>
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

    public BoatAssembly Assembly { get; private set; }



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



    /// <summary>
    /// Set a Hex for the boat to navigate to.
    /// The boat will navigate there after all previous waypoints are set.
    /// </summary>
    /// <param name="waypoint">
    /// The Hex to navigate to.
    /// </param>
    public void AddWaypoint(Hex waypoint)
    {
        MapNavigator.TryNavigateStraight(Waypoints.Last(), waypoint, Waypoints);
    }



    /// <summary>
    /// Clear all waypoints the boat was navigating to.
    /// Stops the boat.
    /// </summary>
    public void ClearWaypoints()
    {
        Waypoints.Clear();

        StopMoving();
    }



    /// <summary>
    /// Start the boat moving.
    /// </summary>
    public void StartMoving()
    {
        Moving = true;
        MoveToNextTarget();
    }



    /// <summary>
    /// Stop the boat from moving.
    /// </summary>
    public void StopMoving()
    {
        Moving = false;
    }


    
    /// <summary>
    /// Adds the boat to the Godot scene tree as a child to the specified node.
    /// Adds all boat parts to the scene tree.
    /// </summary>
    /// <param name="parentNode">
    /// The node the boat will be a child of.
    /// </param>
    public void AddToScene(Node parentNode)
    {
        parentNode.AddChild(visualBoat);

        visualBoat.UpdateBoatParts(Assembly.BoatParts);
    }



    /// <summary>
    /// Remove the VisualBoat from the Godot scene tree.
    /// Removes all boat parts.
    /// </summary>
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
        if (!Moving)
        {
            visualBoat.Moving = false;
        }

        if (Waypoints.TryDequeue(out Hex nextWaypoint))
        {
            visualBoat.Moving = true;

            visualBoat.TargetPos = nextWaypoint.WorldCoordinates();
        }
        else
        {
            Moving = false;

            visualBoat.Moving = false;
            // Maybe need to invoke something for the IController
        }
    }
}
