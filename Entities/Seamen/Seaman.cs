
using Godot;

using Entities.Holds;
using Registries;

namespace Entities.Seamen
{
    /// <summary>
    /// Abstract base class for AbleBody and Officer
    /// </summary>
    public abstract class Seaman : RegisterableEntity, IMortal, IHoldable<IRegisterable>
    {
        public bool Alive { get; private set; } = true;
        public int Age { get; private set; } = 12;
        public int Health  { get; private set; } = 100;

        public int Size { get; } = 1;

        public const int MaxHealth = 100;

        private readonly RandomNumberGenerator rng = new();



        public Seaman()
        {
            Name = SeamenNames.GetName();

            Age = rng.RandiRange(12, 45);
            
            Health = rng.RandiRange(90, 100);
            
        }



        public void Damage(int baseAmount)
        {
            Health -= baseAmount;

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }



        public void Heal(int baseAmount)
        {
            if (!Alive) return;

            Health += baseAmount;

            if (Health > MaxHealth)
                Health = MaxHealth;
        }


        public void Die()
        {
            Alive = false;

            // Maybe need to emit a signal here?
        }



        public override string ToString()
        {
            return Name + ", aged " + Age;
        }
    }
}
