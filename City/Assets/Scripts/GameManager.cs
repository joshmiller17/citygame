using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject cube;
    public GameObject capsule;
    public Material[] defaultMaterials;
    public Material[] happyMaterials;
    public GameObject trigger;

    private void Awake()
    {
        instance = this;
    }



}
