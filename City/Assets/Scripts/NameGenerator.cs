using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator
{
    public TraceryGrammar NPCgrammar;
    public TraceryGrammar Shopgrammar;

    public void Init()
    {
        NPCgrammar = new TraceryGrammar(Resources.Load<TextAsset>("Text/NPCgrammar").text);
        Shopgrammar = new TraceryGrammar(Resources.Load<TextAsset>("Text/Shopgrammar").text);
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
            s.storeName = GetShopName();
        }
    }

    public string GetNPCName() // TODO genders
    {
        return NPCgrammar.Generate();
    }

    public string GetShopName() // TODO shop type
    {
        return Shopgrammar.Generate();
    }
}
