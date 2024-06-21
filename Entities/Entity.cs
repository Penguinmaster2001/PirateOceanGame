
namespace Entities
{
    public abstract class Entity
    {
        public string Name { get; private set; }

        public Entity(string name)
        {
            Name = name;
        }
    }
}
