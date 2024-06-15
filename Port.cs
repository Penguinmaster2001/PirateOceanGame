
using Godot;



/*
 * Contains a bunch of data for an individual port
 * Has a bunch of methods for the PortManager class to call
 */
public class Port
{
    private RandomNumberGenerator rng = new();

    private int name = 0;

    private Hex hex;


    public Port(Hex hex)
    {
        name = rng.RandiRange(1000, 9999);

        this.hex = hex;
    }



    public Vector3 get_spawn_location()
    {
        return hex.get_world_coords();
    }



    public override string ToString()
    {
        return "Port No. " + name;
    }
}
