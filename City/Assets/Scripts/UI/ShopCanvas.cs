using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvas : MonoBehaviour
{
    public static ShopCanvas instance;
    public Shop shop;
    public Texture2D[] itemImages;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static ShopCanvas getInstance()
    {
        return instance;
    }

    public void SetShop(Shop s)
    {
        shop = s;
    }

    Texture2D FindImage(string name)
    {
        for (int i = 0; i < itemImages.Length; i++)
        {
            if (name == itemImages[i].name)
            {
                return itemImages[i];
            }
        }
        throw new System.ArgumentException("Could not find image file: " + name);
    }

    public void SetItem(int index, Item item)
    {
        string boxName = "ItemPanel/ItemBox" + index.ToString();
        GameObject itemBox = transform.Find(boxName).gameObject;
        itemBox.transform.Find("Image").gameObject.GetComponent<RawImage>().texture = FindImage(item.icon);
        itemBox.transform.Find("ItemDesc").gameObject.GetComponent<Text>().text = item.description + "\nCosts $" + item.cost.ToString("F2");
        itemBox.transform.Find("ItemName").gameObject.GetComponent<Text>().text = item.itemName;
        
        if (item.purchased)
        {
            itemBox.transform.Find("Purchased").gameObject.SetActive(true);
        }
        else
        {
            itemBox.transform.Find("Purchased").gameObject.SetActive(false);
        }
    }

    public void Buy(int index)
    {
        if (!shop.inventory[index].purchased)
        {
            if (PlayerController.instance.money >= shop.inventory[index].cost)
            {
                PlayerController.instance.money -= shop.inventory[index].cost;
                shop.Buy(index);
            }
            else
            {
                GameManager.instance.haveThought("I can't afford that.");
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
