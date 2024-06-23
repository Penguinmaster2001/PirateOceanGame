
using Godot;
using HexModule;

namespace Entities
{
    public interface IVisual3D
    {
        Node3D EntityNode { get; }

        void AddToScene(Node parentNode);

        void RemoveFromScene();

        void UpdateVisualPosition(Hex newPosition);
    }
}
