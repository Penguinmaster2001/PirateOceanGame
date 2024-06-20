
using Godot;
using HexModule;



/*
 * Contains a bunch of data for an individual port
 * Has a bunch of methods for the PortManager class to call
 */
public class Port
{
    private RandomNumberGenerator rng = new();

    public Vector3 NewBoatSpawnLocation { get; private set; }

    private readonly int name = 0;

    public Hex PortHex { get; private set; }


    public Port(Hex hex)
    {
        name = rng.RandiRange(1000, 9999);

        PortHex = hex;

        NewBoatSpawnLocation = hex.WorldCoordinates();
    }



    public override string ToString()
    {
        return "Port No. " + name;
    }
}
