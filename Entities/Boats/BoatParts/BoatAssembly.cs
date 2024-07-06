
using System.Collections.Generic;
using System.Linq;



namespace Entities.Boats.BoatParts;



/// <summary>
/// Manages the boat parts in a boat.
/// </summary>
public class BoatAssembly
{
    /// <summary>
    /// All the BoatParts in this assembly
    /// </summary>
    public List<BoatPart> BoatParts { get; private set; } = new();

    public BoatPart RootPart { get; private set; } = null;

    public int Count { get; private set; } = 0;



    /// <summary>
    /// Initialize this BoatAssembly with a root part.
    /// </summary>
    /// <param name="rootPart">
    /// The initial root part.
    /// </param>
    public BoatAssembly(BoatPart rootPart)
    {
        RootPart = rootPart.Clone();

        BoatParts = DiscoverBoatParts();
    }



    /// <summary>
    /// Discover all the BoatParts in this assembly.
    /// </summary>
    /// <returns>
    /// A List<BoatPart> of all the boat parts.
    /// In breadth first order.
    /// </returns>
    private List<BoatPart> DiscoverBoatParts()
    {
        List<BoatPart> discoveredParts = new();

        if (RootPart is null) return discoveredParts;


        // Breadth first search
        Queue<BoatPart> currentParts = new();
        currentParts.Enqueue(RootPart);

        while (currentParts.Count > 0)
        {
            BoatPart currentPart = currentParts.Dequeue();
            discoveredParts.Add(currentPart);

            if (currentPart is CompositeBoatPart compositeBoatPart)
            {
                foreach (BoatPart boatPart in compositeBoatPart.ChildParts)
                    currentParts.Enqueue(boatPart);
            }
        }

        Count = discoveredParts.Count;

        return discoveredParts;
    }



    /// <summary>
    /// Creates a deep copy of this BoatAssembly.
    /// </summary>
    /// <returns>
    /// A new BoatAssembly with cloned parts.
    /// </returns>
    public BoatAssembly Clone()
    {
        // TODO: For large assemblies this might be slow.
        // If so it should be fairly easy to make it faster.
        return new(RootPart.Clone());
    }
}

