
using Godot;



namespace Entities.Boats.BoatParts;



/// <summary>
/// Base class for boat parts like masts, hulls, sails, etc.
/// </summary>
public abstract class BoatPart : Entity, IMortal, IVisual3D
{
    public bool Alive { get; }
    public int Age { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public Node3D EntityNode { get; }
    private readonly VisualBoatPart visualBoatPart;
    public Vector3 PartPosition { get; private set; }



    public BoatPart()
    {
        Alive = true;
        Age = 0;
        Health = 100;
        MaxHealth = 100;

        visualBoatPart = (VisualBoatPart) EntityNode;
    }



    public void UpdatePartPosition(Vector3 value)
    {
        PartPosition = value;
        visualBoatPart.Position = value;
    }



    public void Damage(int baseAmount)
    {
        throw new System.NotImplementedException();
    }



    /// <summary>
    /// Repair the boat part.
    /// </summary>
    /// <param name="baseAmount"></param>
    public void Heal(int baseAmount)
    {
        throw new System.NotImplementedException();
    }



    /// <summary>
    /// Destroy the boat part.
    /// </summary>
    public void Die()
    {
        RemoveFromScene();
    }



    /// <summary>
    /// Add this part to the Godot scene.
    /// </summary>
    /// <param name="parentNode">
    /// The parent of this part in the scene tree.
    /// </param>
    public void AddToScene(Node parentNode)
    {
        parentNode.AddChild(visualBoatPart);
    }



    public void RemoveFromScene()
    {
        visualBoatPart.QueueFree();
    }



    /// <summary>
    /// Creates a deep copy of this part.
    /// </summary>
    /// <returns>
    /// A new BoatPart with cloned sub parts.
    /// </returns>
    public abstract BoatPart Clone();
}
