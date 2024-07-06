
using System;

namespace Entities.Holds;



public class Cargo : Entity, IHoldable, IEquatable<Cargo>
{
    public int Size { get; set; }



    public bool Equals(Cargo cargo)
    {
        return this.Size == cargo.Size;
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
