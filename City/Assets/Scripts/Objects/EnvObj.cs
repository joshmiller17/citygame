using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Environment object, like buildings and other world details
public class EnvObj : MonoBehaviour
{
    public Familiarity fam = new Familiarity();
    public GameObject hideObj;
    public GameObject trigger;

    public virtual void Start()
    {
        Bounds b = GetComponentInChildren<Renderer>().bounds;
        //hideObj = Instantiate(GameManager.instance.capsule, transform);
        //hideObj.transform.SetParent(gameObject.transform);
        //hideObj.GetComponent<MeshRenderer>().material = GameManager.instance.defaultMaterials[Random.Range(0, GameManager.instance.defaultMaterials.Length)];
        //hideObj.transform.localScale = b.extents * 2f;
        //hideObj.transform.position = b.center;

        trigger = Instantiate(GameManager.instance.trigger, transform);
        trigger.transform.SetParent(gameObject.transform);
        trigger.transform.localScale = b.extents * 2f;
        trigger.transform.position = gameObject.transform.position;
    }

    public virtual void Interact()
    {
        if (fam.Interact())
        {
            Debug.Log(string.Format("{0} familiarity is now {1}", gameObject.name, fam.GetFamiliarity()));
            //if (fam.GetFamiliarity() >= 1)
            //{
            //    hideObj.GetComponent<MeshRenderer>().material = GameManager.instance.happyMaterials[Random.Range(0, GameManager.instance.happyMaterials.Length)];
            //}
            //if (fam.GetFamiliarity() >= 2)
            //{
            //    hideObj.SetActive(false);
            //}
        }        
    }
}
