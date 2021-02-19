using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string speech;
    public string characterName;
    public bool talksFirst;
    public Familiarity fam = new Familiarity();

    public string GetSpeech()
    {
        return speech;
    }

    public string GetName()
    {
        return characterName;
    }

    public void Interact()
    {
        fam.Interact();
    }
}
