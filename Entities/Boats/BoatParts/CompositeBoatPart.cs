
using System.Collections.Generic;



namespace Entities.Boats.BoatParts;



/// <summary>
/// This is a boat part that has child boat parts.
/// It also stores the attachment points for them.
/// This should not have any methods for traversing the parts,
/// that's the BoatAssembly's job.
/// </summary>
public abstract class CompositeBoatPart : BoatPart
{
    public List<BoatPart> ChildParts { get; private set; }

    public int Count { get; private set; }

    public abstract override CompositeBoatPart Clone();
}

