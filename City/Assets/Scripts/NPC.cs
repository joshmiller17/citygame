using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string speech;
    public string characterName;
    public bool talksFirst;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetSpeech()
    {
        return speech;
    }

    public string GetName()
    {
        return characterName;
    }
}
