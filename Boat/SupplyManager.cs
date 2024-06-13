using Godot;
using System;

public class SupplyManager
{
	// food, drinkable water, cannonballs, currency, tar

    private int num_cannonballs = 0;

    private int num_coins = 0;

    private float barrels_of_food = 0.0f;

    private float barrels_of_grog = 0.0f;

    // Resource used to repair ships
    private float barrels_of_tar = 0.0f;

    public SupplyManager()
    {

    }



    public override string ToString()
    {
        return "Supplies:"
            + "\nCoins: " + num_coins
            + "\nCannonballs: " + num_cannonballs
            + "\nBarrels of food: " + barrels_of_food
            + "\nBarrels of grog: " + barrels_of_grog
            + "\nBarrels of tar: " + barrels_of_tar;
    }
}
