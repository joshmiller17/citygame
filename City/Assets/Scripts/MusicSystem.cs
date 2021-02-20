using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public MusicSystem instance;
    public string SongDir;
    public string RhythmDir;
    public Song song;
    public GameObject BeatSpawner;

    void Start()
    {
        instance = this;
    }

    public Song LoadSong(string name, int beatDifficulty, float speedDifficulty)
    {
        Rhythm r = new Rhythm(RhythmDir + "/" + name);
        Song s = new Song();
        s.rhythm = r;
        Debug.Log("Loading music " + SongDir + "/" + name);
        s.clip = Resources.Load<AudioClip>(SongDir + "/" + name);
        Debug.Log(s.clip);
        s.speedDifficulty = speedDifficulty;
        s.beatDifficulty = beatDifficulty;
        s.name = name;
        return s;
    }

    public void SetSong(Song s)
    {
        song = s;
        gameObject.GetComponent<AudioSource>().clip = song.clip;
        Debug.Log(s.clip);
        BeatSpawner.GetComponent<BeatSpawner>().SetSong(song);
    }

    public void Play()
    {
        Debug.Log("Now playing: " + song.name);
        Debug.Log(song.clip);
        gameObject.GetComponent<AudioSource>().Play();
        BeatSpawner.GetComponent<BeatSpawner>().Play();
    }
}
