using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : EnvObj
{
    // TODO spawning other EnvObjs
    public GameObject[] extraHideObjs;

    public override void Start()
    {
        hideObj = Instantiate(GameManager.instance.cube, transform);
        Bounds b = GetComponentInChildren<Renderer>().bounds;
        hideObj.transform.SetParent(gameObject.transform);
        hideObj.transform.localScale = b.extents * 1.05f;
        hideObj.GetComponent<MeshRenderer>().material = GameManager.instance.defaultMaterials[Random.Range(0, GameManager.instance.defaultMaterials.Length)];
        hideObj.transform.position = b.center;

        //TODO show extraHideObjs

        trigger = Instantiate(GameManager.instance.trigger, transform);
        trigger.transform.SetParent(gameObject.transform);
        trigger.transform.localScale = b.extents * 2f;
        trigger.transform.position = b.center;

     
    }


}
