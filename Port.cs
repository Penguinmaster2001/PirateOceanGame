
using Godot;



/*
 * Contains a bunch of data for an individual port
 * Has a bunch of methods for the PortManager class to call
 */
public class Port
{
    private RandomNumberGenerator rng = new();

    private int name = 0;


    public Port()
    {
        name = rng.RandiRange(1000, 9999);
    }



    public override string ToString()
    {
        return "Port No. " + name;
    }
}
