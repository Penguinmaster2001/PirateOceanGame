
using System;

namespace Entities.Holds
{
    /// <summary>
    /// Represents an item that can be held in a Entities.Holds.Hold.
    /// </summary>
    public interface IHoldable<T> : IEquatable<T>
    {
        /// <summary>
        /// The size of the holdable item.
        /// </summary>
        int Size { get; }
    }
}