using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Environment object, like buildings and other world details
public class EnvObj : MonoBehaviour
{
    public Familiarity fam = new Familiarity();
    public GameObject trigger;
    public Color hideColor = Color.yellow;

    public virtual void Start()
    {
        Bounds b = GetComponentInChildren<Renderer>().bounds;

        trigger = Instantiate(GameManager.instance.trigger, transform);
        trigger.transform.SetParent(gameObject.transform);
        trigger.transform.localScale = b.extents + new Vector3(100,100,100);
        trigger.transform.position = gameObject.transform.position;

        if (GameManager.instance.DebugMode)
        {
            fam.SetFamiliarity(99);
            ToggleHideObj(false);
        }
        else
        {
            ToggleHideObj(true);
        }
    }

    void ToggleHideObj(bool hide)
    {
        Transform hideTrans = transform.Find("Hide");
        if (hideTrans == null)
        {
            Debug.Log(string.Format("{0} has no Hide objects, removing this asset", gameObject.name));
            Destroy(gameObject);
        }
        else
        {
            hideTrans.gameObject.SetActive(hide);
        }
    }

    void ColorHideObj(Color c)
    {
        Transform hideTrans = transform.Find("Hide");
        if (hideTrans == null)
        {
            Debug.Log(string.Format("{0} has no Hide objects, removing this asset", gameObject.name));
            Destroy(gameObject);
        }
        else
        {
            foreach (Transform t in hideTrans.gameObject.GetComponentsInChildren<Transform>())
            {
                if (t != hideTrans)
                {
                    t.gameObject.GetComponent<MeshRenderer>().material.color = c;
                }
            }
        }
    }

    public virtual void Interact()
    {
        if (fam.Interact())
        {
            Debug.Log(string.Format("{0} familiarity is now {1}", gameObject.name, fam.GetFamiliarity()));
            if (fam.GetFamiliarity() >= 1)
            {
                ColorHideObj(hideColor);
            }
            if (fam.GetFamiliarity() >= 2)
            {
                ToggleHideObj(false);
            }
        }        
    }
}
