
namespace Entities.Holds
{
    /// <summary>
    /// Represents an item that can be held in a Entities.Holds.Hold.
    /// Classes that implement this should also implement IEquatable
    /// </summary>
    public interface IHoldable
    {
        /// <summary>
        /// The size of the holdable item.
        /// </summary>
        int Size { get; }
    }
}