
using Registries;

namespace Entities
{
    /// <summary>
    /// This is for entities that need to be unique
    /// </summary>
    public abstract class RegisterableEntity : Entity, IRegisterable
    {
        public int ID { get; private set; }

        public RegisterableEntity()
        {
            ID = GlobalRegistry.Register(this);
        }



        public bool Equals(IRegisterable other)
        {
            return other != null && ID == other.ID;
        }



        public override bool Equals(object obj)
        {
            if (obj is not RegisterableEntity registerableEntity) return false;

            return Equals(registerableEntity);
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
