using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Item[] inventory;
    public float priceModifier;
    public float valueModifier;
    public float randomness;
    public int daysUntilClose;

    // Start is called before the first frame update
    void Start()
    {
        priceModifier = 1.2f;
        valueModifier = 1.1f;
        randomness = 0.2f;

        inventory = new Item[3];
        inventory[0].cost = 10;
        inventory[1].cost = 20;
        inventory[2].cost = 30;
        inventory[0].value = 1;
        inventory[1].value = 2;
        inventory[2].value = 3;
        inventory[0].itemName = "burger";
        inventory[1].itemName = "shake";
        inventory[2].itemName = "power drink";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // called when player enters shop
    public void Show()
    {

    }

}
