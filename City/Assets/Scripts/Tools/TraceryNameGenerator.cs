using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomNameGeneratorLibrary;
using static Shop;

public class TraceryNameGenerator : MonoBehaviour
{
    public TraceryGrammar NPCgrammar;
    public TraceryGrammar Shopgrammar;
    public TextAsset NPCText;
    public TextAsset ShopText;
    public PersonNameGenerator nameGenerator;

    public void Init()
    {
        NPCgrammar = new TraceryGrammar(NPCText.text);
        Shopgrammar = new TraceryGrammar(ShopText.text);
        nameGenerator = new PersonNameGenerator();
        NameNPCs();
        NameShops();
    }

    void NameNPCs()
    {
        GameObject[] npcs;
        npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in npcs)
        {
            NPC n = npc.GetComponent<NPC>();
            n.characterName = GetNPCName();
        }
    }

    void NameShops()
    {
        GameObject[] shops;
        shops = GameObject.FindGameObjectsWithTag("Shop");
        foreach (GameObject shop in shops)
        {
            Shop s = shop.GetComponent<Shop>();
            s.storeName = GetShopName(s.shopType);
        }
    }

    public string GetNPCName() // TODO genders???
    {
        return nameGenerator.GenerateRandomFirstAndLastName();
        //return NPCgrammar.Generate();
    }

    public string GetShopName(ShopType t)
    {
        return Shopgrammar.Generate(null, t.ToString());
    }
}
