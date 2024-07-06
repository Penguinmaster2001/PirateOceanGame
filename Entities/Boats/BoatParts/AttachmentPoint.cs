
using Godot;



namespace Entities.Boats.BoatParts;



public class AttachmentPoint<T> where T : BoatPart
{
    public Vector3 Position { get; private set; }


    public AttachmentPoint(Vector3 position)
    {
        Position = position;
    }
}
