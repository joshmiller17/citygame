using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public enum ShopType { Food, Junk, Energy, Coffee, Travel, Music};

    public Item[] inventory = new Item[3];
    public float priceModifier;
    public float valueModifier;
    public float shopRandomness; // in percentage of potential variance, set once
    public int daysUntilClose;
    public string storeName = "Test Store";
    public ShopType shopType;

    
    private float itemRandomness; // in percentage of potential variance, rolled once per item
    private int dateRestocked = 0;

    // Start is called before the first frame update
    void Start()
    {
        //test numbers set
        priceModifier = 1.2f;
        valueModifier = 1.1f;
        shopRandomness = 0.2f;
        shopType = ShopType.Food;

        //actual rolling
        itemRandomness = Random.Range(-1f * shopRandomness, shopRandomness);
        shopRandomness = Random.Range(-1f * shopRandomness, shopRandomness);

        Debug.Log(string.Format(
            "Made {6} shop \"{4}\" closing in {5} with these modifiers: price {0}\nvalue {1}\nshop {2}\nitem {3}\n", 
            priceModifier.ToString("F2"), valueModifier.ToString("F2"), 
            shopRandomness.ToString("F2"), itemRandomness.ToString("F2"), storeName, 
            daysUntilClose.ToString(), shopType.ToString()));

        ResetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    FoodItem GenerateFoodItem(FoodItem.FoodType type, float itemRand)
    {
        FoodItem f = new FoodItem();
        f = FoodItem.BaseItem(type);
        f.value *= valueModifier * itemRand;
        f.cost *= priceModifier * itemRand;
        bool negFlip = Random.value < .5;
        f.food *= f.food < 0 && negFlip ? itemRand : 1.0f / itemRand;
        f.health *= f.health < 0 && negFlip ? itemRand : 1.0f / itemRand;
        f.energy *= f.energy < 0 && negFlip ? itemRand : 1.0f / itemRand;

        switch (type)
        {
            case FoodItem.FoodType.Snack:
                f.icon = "snack";
                f.itemName = "Snack";
                f.description = "A tasty snack.";
                break;

            case FoodItem.FoodType.Meal:
                f.icon = "meal";
                f.itemName = "Meal";
                f.description = "A delicious meal.";
                break;

            case FoodItem.FoodType.JunkSnack:
                f.icon = "junksnack";
                f.itemName = "Snack";
                f.description = "A tasty but not very nutritious snack.";
                break;

            case FoodItem.FoodType.JunkMeal:
                f.icon = "junkmeal";
                f.itemName = "Meal";
                f.description = "A cheap but not very nutritious meal.";
                break;

            case FoodItem.FoodType.EnergyDrink:
                f.icon = "energy";
                f.itemName = "Energy Drink";
                f.description = "A jolt of energy.";
                break;
        }
        return f;
    }

    void ResetInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            float itemRand = 1 + Random.Range(-1f * itemRandomness, itemRandomness) * shopRandomness;
            FoodItem.FoodType foodType;
            switch (shopType)
            {
                case ShopType.Food:
                    foodType = Random.value < .5 ? FoodItem.FoodType.Snack : FoodItem.FoodType.Meal;
                    inventory[i] = GenerateFoodItem(foodType, itemRand);
                    break;
                case ShopType.Junk:
                    foodType = Random.value < .5 ? FoodItem.FoodType.JunkSnack : FoodItem.FoodType.JunkMeal;
                    inventory[i] = GenerateFoodItem(foodType, itemRand);
                    break;
                case ShopType.Energy:
                    foodType = FoodItem.FoodType.EnergyDrink;
                    inventory[i] = GenerateFoodItem(foodType, itemRand);
                    break;



                case ShopType.Coffee:
                    //TODO
                    break;
                case ShopType.Travel:
                    //TODO
                    break;
                case ShopType.Music:
                    //TODO
                    break;
            }
        }
    }


    public void Buy(int index)
    {
        if (!inventory[index].purchased)
        {
            inventory[index].Use();
            inventory[index].purchased = true;
            ShopCanvas.instance.SetItem(index, inventory[index]);
        }
    }

    // called when player enters shop
    public void Show(int day)
    {
        if (day != dateRestocked)
        {
            ResetInventory();
            dateRestocked = day;
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            ShopCanvas.instance.SetItem(i, inventory[i]);
        }
        ShopCanvas.instance.SetShop(this);
        ShopCanvas.instance.gameObject.SetActive(true);
    }

}
