using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatReceiver : MonoBehaviour
{
    public BeatSpawner Spawner;
    public AudioSource Hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Spawner.Beats.Count - 1 > 0 && Spawner.IsActive) {
            if (Input.GetMouseButtonDown(0))
            {
                BeatController LastBeat = Spawner.Beats[0].GetComponent<BeatController>();
                //Debug.Log("Last beat has position " + LastBeat.transform.position.x);
                float ActualPosition = LastBeat.transform.position.x;

                if (Mathf.Abs(LastBeat.PerfectNote - ActualPosition) < LastBeat.NoteDeviation / 2)
                {
                    Debug.Log("Excellent");
                    LastBeat.DeleteBeat();
                }
                else if (Mathf.Abs(LastBeat.PerfectNote - ActualPosition) < LastBeat.NoteDeviation)
                {
                    Debug.Log("OK");
                    LastBeat.DeleteBeat();
                }
                else
                {
                    Debug.Log("Miss");
                }
                Hit.Play();
            }
        }
    }
}
