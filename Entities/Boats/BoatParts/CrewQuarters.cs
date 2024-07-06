
using Entities.Holds;



namespace Entities.Boats.BoatParts;



public class CrewQuarters : BoatPart
{
    public CrewHold Crew { get; }



    public CrewQuarters(CrewHold initialCrew)
    {
        Crew = initialCrew;
    }



    public override CrewQuarters Clone()
    {
        return new(Crew.Clone());
    }
}

