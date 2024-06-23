
namespace HexModule.Map
{
    // Interface for anything that needs to be displayed on the Hex Map
    public interface IMappable
    {
        Hex MapPosition { get; }
    }
}
