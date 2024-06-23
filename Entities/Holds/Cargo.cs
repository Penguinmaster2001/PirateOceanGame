
namespace Entities.Holds
{
    public class Cargo : Entity, IHoldable<Cargo>
    {
        public int Size { get; set; }



        public bool Equals(Cargo cargo)
        {
            return this.Equals(cargo);
        }



        public override bool Equals(object obj)
        {
            if (obj is not Cargo cargo) return false;

            return this.Equals(cargo);
        }



        public static bool operator ==(Cargo left, Cargo right)
        {
            if (left is null) return right is null;

            return left.Equals(right);
        }



        public static bool operator !=(Cargo left, Cargo right)
        {
            return !(left == right);
        }



        public override int GetHashCode()
        {
            return 0;
        }
    }
}
