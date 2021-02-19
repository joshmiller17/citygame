using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Environment object, like buildings and other world details
public class EnvObj : MonoBehaviour
{
    public Familiarity fam = new Familiarity();

    public void Interact()
    {
        fam.Interact();
        Debug.Log(string.Format("EnvObj familiarity is now {0}", fam.GetFamiliarity()));
    }
}
