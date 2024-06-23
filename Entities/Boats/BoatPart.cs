
using Godot;

using HexModule;



namespace Entities.Boats;



/// <summary>
/// Base class for boat parts like masts, hulls, sails, etc.
/// </summary>
public abstract class BoatPart : Entity, IMortal, IVisual3D
{
    public bool Alive { get; }
    public int Age { get; }
    public int Health { get; }
    public Node3D EntityNode { get; }



    public void Damage(int baseAmount)
    {
        throw new System.NotImplementedException();
    }



    public void Die()
    {
        throw new System.NotImplementedException();
    }



    public void Heal(int baseAmount)
    {
        throw new System.NotImplementedException();
    }



    public void AddToScene(Node parentNode)
    {
        throw new System.NotImplementedException();
    }



    public void RemoveFromScene()
    {
        throw new System.NotImplementedException();
    }



    public void UpdateVisualPosition(Hex newPosition)
    {
        throw new System.NotImplementedException();
    }
}
