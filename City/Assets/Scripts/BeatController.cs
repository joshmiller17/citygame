using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    public bool IsMirror = false;
    private bool IsActive = true;

    private GameObject Mirror;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsMirror)
        {
            Mirror = Instantiate(BeatSpawner.instance.BeatPrefab, transform);
            Mirror.transform.SetParent(gameObject.transform);
            Mirror.GetComponent<BeatController>().IsMirror = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMirror)
        {
            float moveDistance = 0.1f * Time.deltaTime * BeatSpawner.instance.BeatSpeed * Screen.width;
            transform.position = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);

            if (transform.position.x > BeatSpawner.instance.transform.position.x)
            {
                transform.localScale = new Vector3(0, 0, 0); //invisible but still active
            }

            if (transform.position.x > BeatSpawner.instance.transform.position.x + BeatSpawner.instance.ScreenPercentForGood)
            {
                if (IsActive)
                {
                    BeatSpawner.instance.MissBeat();
                }
                DeleteBeat();
            }

            //also move mirror
            Mirror.transform.position = new Vector3(Screen.width - transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            //Mirror
        }
    }

    public void DeleteBeat()
    {
        BeatSpawner.instance.Beats.Remove(gameObject);
        //Debug.Log("Beat faded!");
        Destroy(gameObject);
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        if (!IsActive)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
