
using Godot;



namespace Entities;



public interface IVisual3D
{
    Node3D EntityNode { get; }

    void AddToScene(Node parentNode);

    void RemoveFromScene();

    // void UpdateVisualPosition(Vector3 newPosition);
}
