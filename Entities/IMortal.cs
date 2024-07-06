
namespace Entities;



/// <summary>
/// Interface for anything that can be damaged or die / be destroyed
/// </summary>
public interface IMortal
{
    /// <summary>
    /// True if this is alive.
    /// False if this is dead.
    /// </summary>
    bool Alive { get; }


    /// <summary>
    /// The age in years of this.
    /// </summary>
    int Age { get; }


    /// <summary>
    /// How much health this has left.
    /// </summary>
    int Health { get; }

    
    /// <summary>
    /// The maximum that Health can be.
    /// </summary>
    int MaxHealth { get; }


    // TODO: Does this interface need this?
    // public event EventHandler died;



    /// <summary>
    /// Apply damage.
    /// </summary>
    /// <param name="baseAmount">
    /// The amount of damage applied.
    /// The amount of actual health removed or gained should be based on this.
    /// </param>
    void Damage(int baseAmount);




    /// <summary>
    /// Heal.
    /// </summary>
    /// <param name="baseAmount">
    /// The amount of healing applied.
    /// The amount of actual health gained should be based on this.
    /// </param>
    void Heal(int baseAmount);



    /// <summary>
    /// Kill this.
    /// </summary>
    void Die();
}
