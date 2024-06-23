
using System.Linq;
using System.Collections.Generic;

namespace Entities.Holds
{
    public abstract class Hold<T> where T : IHoldable<T>
    {
        public int Capacity { get; private set; }

        public int UsedCapacity { get; private set;}

        public HashSet<T> Contents { get; private set; } = new();


        public Hold(int capacity)
        {
            Capacity = capacity;
        }



        public bool CanStore(T holdable)
        {
            if (holdable.Size + UsedCapacity > Capacity) return false;

            if (!IsStored(holdable)) return false;

            return true;
        }
        
        public bool TryStore(T holdable)
        {
            if (!CanStore(holdable)) return false;

            Contents.Add(holdable);

            return true;
        }



        public bool IsStored(T holdable)
        {
            return Contents.Contains(holdable);
        }


        
        public bool TryRemove(T holdable)
        {
            if (!IsStored(holdable)) return false;

            Contents.Remove(holdable);

            UsedCapacity -= holdable.Size;

            return true;
        }



        public bool IsFull()
        {
            return UsedCapacity == Capacity;
        }



        public bool IsEmpty()
        {
            return Contents.Count == 0;
        }


        /// <summary>
        /// Checks if something of a specific type is stored.
        /// For example: if an IHold<ShipPart> has any Mast.
        /// </summary>
        public bool ContainsType<S>() where S : T
        {
            return Contents.Any(holdable => holdable is S);
        }
    }
}
