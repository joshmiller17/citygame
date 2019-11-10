using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    public float speed;
    public BeatSpawner ParentSpawner;
    public int PerfectNote = 200;
    public int NoteDeviation = 50;
    private bool IsActive = true;
    private bool IsReal = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed, transform.position.y, transform.position.z);
        if (transform.position.x > PerfectNote + NoteDeviation)
        {
            DeleteBeat();
        }
    }

    public void DeleteBeat()
    {
        ParentSpawner.Beats.Remove(gameObject);
        //Debug.Log("Beat faded!");
        Destroy(gameObject);
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        if (IsActive)
        {
            IsReal = false; //when resuming play, pseudo-delete the existing beats to give the player time to get up to speed
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
