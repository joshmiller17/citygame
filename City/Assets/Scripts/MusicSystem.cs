using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public MusicSystem instance;
    public Song song;
    public GameObject BeatSpawner;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetSong(Song s)
    {
        song = s;
        gameObject.GetComponent<AudioSource>().clip = song.clip;
        BeatSpawner.GetComponent<BeatSpawner>().BeatRhythm = song.rhythm;
    }
}
