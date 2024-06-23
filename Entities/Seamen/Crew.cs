
using Entities.Holds;

namespace Entities.Seamen
{
    public class Crew : Hold<Seaman>
    {
        public Crew(int capacity) : base(capacity)
        {

        }
    }
}