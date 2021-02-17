using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Environment object, like buildings and other world details
public class EnvObj : MonoBehaviour
{
    public Familiarity fam = new Familiarity();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
    }
}
