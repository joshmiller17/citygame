using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Item
{
    public enum ItemType { Food, Coffee, Travel, Music };

    public float cost;
    public float value; //0-5; overall value
    public string icon;
    public string itemName;
    public string description;
    public ItemType type;
    public bool purchased;


    void Awake()
    {
        value = 1;
        purchased = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public virtual void Use()
    {
    }

}

public class FoodItem : Item
{
    public float food;
    public float health;
    public float energy;

    public enum FoodType { Snack, Meal, EnergyDrink, JunkSnack, JunkMeal };
    
    void Awake()
    {
        type = ItemType.Food;
    }

    public static FoodItem BaseItem(FoodType type)
    {
        FoodItem ret = new FoodItem();
        switch (type)
        {
            case FoodType.Snack:
                ret.food = 2;
                ret.health = 1;
                ret.energy = 1;
                ret.cost = 10;
                break;

            case FoodType.Meal:
                ret.food = 10;
                ret.health = 5;
                ret.energy = -5;
                ret.cost = 30;
                break;

            case FoodType.JunkSnack:
                ret.food = 5;
                ret.health = -5;
                ret.energy = 5;
                ret.cost = 10;
                break;

            case FoodType.JunkMeal:
                ret.food = 20;
                ret.health = -5;
                ret.energy = -10;
                ret.cost = 20;
                break;

            case FoodType.EnergyDrink:
                ret.food = 0;
                ret.health = -1;
                ret.energy = 10;
                ret.cost = 20;
                break;

        }
        return ret;
    }

    public override void Use()
    {
        PlayerController.instance.food += food;
        PlayerController.instance.health += health;
        PlayerController.instance.energy += energy;
    }
}

public class CoffeeItem : Item
{
    public float boost;
    public float boostDuration;
    public float jumpMultiplier;

    public enum CoffeeType { Coffee, LongCoffee, JumpCoffee};

    void Awake()
    {
        type = ItemType.Coffee;
    }

    public static CoffeeItem BaseItem(CoffeeType type)
    {
        CoffeeItem ret = new CoffeeItem();
        switch (type)
        {
            case CoffeeType.Coffee:
                ret.boost = 1;
                ret.boostDuration = 60;
                ret.jumpMultiplier = -0.5f;
                ret.cost = 10;
                break;

            case CoffeeType.LongCoffee:
                ret.boost = 0.5f;
                ret.boostDuration = 120;
                ret.jumpMultiplier = -0.5f;
                ret.cost = 20;
                break;

            case CoffeeType.JumpCoffee:
                ret.boost = 0.1f;
                ret.boostDuration = 30;
                ret.jumpMultiplier = 2.0f;
                ret.cost = 15;
                break;

        }
        return ret;
    }

    public override void Use()
    {
        PlayerController.instance.speedBoost = boost;
        PlayerController.instance.boostDuration = boostDuration;
        PlayerController.instance.jumpMultiplier = jumpMultiplier;
    }
}

public class TravelItem : Item
{
    public float acceleration;
    public float maxSpeed;
    public float jumpHeight;

    public enum TravelType { Run, Skate, Scoot, Wheels };


    void Awake()
    {
        type = ItemType.Travel;
    }

    public static TravelItem BaseItem(TravelType type)
    {
        TravelItem ret = new TravelItem();
        switch (type)
        {
            case TravelType.Run:
                ret.acceleration = 3;
                ret.maxSpeed = 1;
                ret.jumpHeight = 2;
                break;

            case TravelType.Skate:
                ret.acceleration = 2;
                ret.maxSpeed = 2;
                ret.jumpHeight = 0.5f;
                break;

            case TravelType.Scoot:
                ret.acceleration = 1;
                ret.maxSpeed = 3;
                ret.jumpHeight = 0.5f;
                break;

            case TravelType.Wheels:
                ret.acceleration = 2;
                ret.maxSpeed = 2;
                ret.jumpHeight = 1;
                break;

        }
        return ret;
    }

    public override void Use()
    {

    }
}

public class MusicItem : Item
{
    public Color color;
    public Song song;

    void Awake()
    {
        type = ItemType.Music;
    }

    public static MusicItem BaseItem()
    {
        MusicItem ret = new MusicItem();
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Audio/Music/");
        FileInfo[] info = dir.GetFiles("*");
        string randomSong = Path.GetFileNameWithoutExtension(info[Random.Range(0, info.Length)].Name);
        randomSong = Path.GetFileNameWithoutExtension(randomSong); // cut off both extensions, e.g. .mp3.meta
        Debug.Log("Randomly picked song " + randomSong);
        ret.song = MusicSystem.instance.LoadSong(randomSong, 1, .05f);
        ret.cost = 100;
        ret.color = new Color(
         Random.Range(0f, 1f),
         Random.Range(0f, 1f),
         Random.Range(0f, 1f)
        );
        return ret;
    }

    public override void Use()
    {
        Inventory.instance.musicItems.Add(this);
    }
}