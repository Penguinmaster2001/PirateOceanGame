
using System.Linq;
using System.Collections.Generic;

using Entities.Seamen;



namespace Entities.Holds;



public class CrewHold : Hold<Seaman>
{
    public CrewHold(int capacity) : base(capacity)
    {

    }


    
    public CrewHold(int capacity, HashSet<Seaman> seamen) : base(capacity, seamen)
    {

    }



    public override CrewHold Clone()
    {
        return new(Capacity, Contents.ToHashSet());
    }
}