using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Building
{
    public enum ShopType {Any, Food, Junk, Energy, Coffee, Music, Travel};

    public Item[] inventory = new Item[3];
    [Range(0.3f, 3f)] public float priceModifier = 1;
    [Range(0.3f, 3f)] public float valueModifier = 1;
    [Range(0f, 3f)] public float shopRandomness = 1; // in percentage of potential variance, set once
    
    public ShopType shopType;

    [Header("Set by generator")]
    public int daysUntilClose;
    public string storeName = "Test Store";

    private float itemRandomness; // in percentage of potential variance, rolled once per item
    private int dateRestocked = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        GameObject trigger = transform.Find("Trigger(Clone)").gameObject;
        trigger.name = "ShopTrigger";

        daysUntilClose = Random.Range(1, 9);

        if (shopType == ShopType.Any)
        {
            shopType = (ShopType)Random.Range(0, 5); // TODO FIXME add more range as I write and test shops
        }

        //actual rolling
        itemRandomness = Random.Range(-1f * shopRandomness, shopRandomness);
        shopRandomness = Random.Range(-1f * shopRandomness, shopRandomness);

        Debug.Log(string.Format(
            "Made {6} shop \"{4}\" closing in {5} with these modifiers: price {0}, value {1}, shop {2}, item {3}", 
            priceModifier.ToString("F2"), valueModifier.ToString("F2"), 
            shopRandomness.ToString("F2"), itemRandomness.ToString("F2"), storeName, 
            daysUntilClose.ToString(), shopType.ToString()));

        ResetInventory();
    }

    FoodItem GenerateFoodItem(FoodItem.FoodType type, float itemRand)
    {
        FoodItem f = new FoodItem();
        f = FoodItem.BaseItem(type);
        f.value *= valueModifier * itemRand;
        f.cost *= priceModifier * itemRand;
        f.cost = Mathf.Round(f.cost * 100f) / 100f; //Round to 2 places
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

    CoffeeItem GenerateCoffeeItem(CoffeeItem.CoffeeType type, float itemRand)
    {
        CoffeeItem c = new CoffeeItem();
        c = CoffeeItem.BaseItem(type);
        c.value *= valueModifier * itemRand;
        c.cost *= priceModifier * itemRand;
        c.cost = Mathf.Round(c.cost * 100f) / 100f; //Round to 2 places
        bool negFlip = Random.value < .5;
        c.boost *= c.boost < 0 && negFlip ? itemRand : 1.0f / itemRand;
        c.boostDuration *= c.boostDuration < 0 && negFlip ? itemRand : 1.0f / itemRand;
        c.jumpBoost *= c.jumpBoost < 0 && negFlip ? itemRand : 1.0f / itemRand;

        switch (type)
        {
            case CoffeeItem.CoffeeType.Coffee:
                c.icon = "coffee";
                c.itemName = "Coffee";
                c.description = "A classic coffee.";
                break;

            case CoffeeItem.CoffeeType.LongCoffee:
                c.icon = "longcoffee";
                c.itemName = "Long Coffee";
                c.description = "For a long-lasting boost.";
                break;

            case CoffeeItem.CoffeeType.JumpCoffee:
                c.icon = "jumpcoffee";
                c.itemName = "Jump Coffee";
                c.description = "Hop higher with this coffee with a kick!";
                break;
        }
        return c;
    }

    MusicItem GenerateMusicItem(float itemRand)
    {
        MusicItem m = new MusicItem();
        m = MusicItem.BaseItem();
        m.value *= valueModifier * itemRand;
        m.cost *= priceModifier * itemRand;
        m.song.beatDifficulty = 1 + (m.value / 2f); // value typically has range of 0-6, difficulty wants 1-4
        m.song.speedDifficulty += Random.Range(-0.05f / Mathf.Max(1, m.value), 0.1f * m.value); // generally lower speed for lower difficulty
        m.song.speedDifficulty = Mathf.Max(0.001f * m.value, m.song.speedDifficulty); // clamp
        m.cost = Mathf.Round(m.cost * 100f) / 100f; //Round to 2 places
        m.icon = "headphones";
        m.itemName = m.song.name;
        m.description = string.Format("Difficulty: {0}\nSpeed: {1}", m.song.beatDifficulty, (100 * m.song.speedDifficulty).ToString("F1"));
        return m;
    }

    void ResetInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            float itemRand = 1 + Random.Range(-1f * itemRandomness, itemRandomness) * shopRandomness;
            FoodItem.FoodType foodType;
            CoffeeItem.CoffeeType coffeeType;
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
                    coffeeType = Random.value < .4 ? CoffeeItem.CoffeeType.Coffee : CoffeeItem.CoffeeType.LongCoffee;
                    if (coffeeType == CoffeeItem.CoffeeType.LongCoffee)
                    {
                        coffeeType = Random.value < .5 ? CoffeeItem.CoffeeType.JumpCoffee : CoffeeItem.CoffeeType.LongCoffee;
                    }
                    inventory[i] = GenerateCoffeeItem(coffeeType, itemRand);
                    break;
                case ShopType.Travel:
                    //TODO
                    break;
                case ShopType.Music:
                    inventory[i] = GenerateMusicItem(itemRand);
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
            Debug.Log("Restocking");
            ResetInventory();
            dateRestocked = day;
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            ShopCanvas.instance.SetItem(i, inventory[i]);
        }
        ShopCanvas.instance.SetShop(this);
        ShopCanvas.instance.gameObject.SetActive(true);

        // familiarity
        EnvObj envObj = gameObject.GetComponentInParent<EnvObj>();
        if (envObj != null)
        {
            envObj.Interact();
        }
    }
}
