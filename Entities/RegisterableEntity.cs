
using Registries;

namespace Entities
{
    public abstract class RegisterableEntity : Entity, IRegisterable
    {
        public int ID { get; private set; }

        public RegisterableEntity(string name) : base(name)
        {
            ID = GlobalRegistry.Register(this);
        }



        public override bool Equals(object obj)
        {
            if (obj is not RegisterableEntity registerableEntity) return false;

            return Equals(registerableEntity);
        }



        public bool Equals(IRegisterable other)
        {
            return other != null && ID == other.ID;
        }



        public static bool operator ==(RegisterableEntity left, RegisterableEntity right)
        {
            if (left is null) return right is null;

            return left.Equals(right);
        }



        public static bool operator !=(RegisterableEntity left, RegisterableEntity right)
        {
            return !(left == right);
        }



        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
