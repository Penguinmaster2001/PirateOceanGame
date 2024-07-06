
namespace Entities.Holds;



public class CargoHold : Hold<Cargo>
{
    public CargoHold(int capacity) : base(capacity)
    {
        
    }



    public override CargoHold Clone()
    {
        throw new System.NotImplementedException();
    }
}
