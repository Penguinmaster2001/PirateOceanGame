
namespace Entities
{
    // Interface for anything that can be damaged or die (Including boat parts I guess?)
    public interface IMortal
    {
        bool Alive { get; }

        int Age { get; }

        int Health { get; }

        // Signal died(IMortal this)?


        // The amount of actual health removed or gained can be modified
        void Damage(int baseAmount);

        void Heal(int baseAmount);

        void Die();
    }
}