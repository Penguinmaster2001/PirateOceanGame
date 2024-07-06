
using System;



namespace Registries
{
    public interface IRegisterable : IEquatable<IRegisterable>
    {
        int ID { get; }
    }
}
