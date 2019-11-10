using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    public GameObject BeatPrefab;
    public float BeatInterval = 1;
    public List<GameObject> Beats;
    public bool IsActive = true;

    private float ToNextBeat;


    // Start is called before the first frame update
    void Start()
    {
        ToNextBeat = BeatInterval;
    }

    // Update is called once per frame
    void Update()
    {
        ToNextBeat -= Time.deltaTime;
        if (ToNextBeat < 0)
        {
            SpawnBeat();
            ToNextBeat = BeatInterval;
        }
    }

    void SpawnBeat()
    {
        GameObject NewBeat = Instantiate(BeatPrefab, transform);
        NewBeat.transform.SetParent(gameObject.transform);
        NewBeat.GetComponent<BeatController>().ParentSpawner = this;
        if (!IsActive)
        {
            NewBeat.GetComponent<BeatController>().Toggle();
        }
        Beats.Add(NewBeat);
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        foreach (GameObject beat in Beats)
        {
            beat.GetComponent<BeatController>().Toggle();
        }
    }
}
