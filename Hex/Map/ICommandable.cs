
namespace HexModule.Map
{
    /// <summary>
    /// Interface for anything that can be commanded by an ICommander
    /// Really for anything that moves across the map: boats, fish, weather, monsters
    /// </summary>
    public interface ICommandable : IMappable
    {
        bool Moving { get; }

        float MovementSpeed { get; }


        void AddWaypoint(Hex waypoint);


        void StartMoving();


        void StopMoving();
    }
}
