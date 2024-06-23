
using Entities.Holds;

namespace Entities
{
    public class CargoHold : Hold<Cargo>
    {
        public CargoHold(int capacity) : base(capacity)
        {
        }
    }
}
