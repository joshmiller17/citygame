using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public float SecondsBeforeReplay;
    public static MusicSystem instance;
    public AudioClip[] MusicFiles;
    public TextAsset[] Rhythms;
    public Song song;
    public GameObject BeatSpawner;

    private float timeUntilReplay = 0f;
    private bool preparingReplay = false;

    private Song lastSongPlayed;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!GameManager.instance.MusicOn) return;
        if (preparingReplay)
        {
            timeUntilReplay -= Time.deltaTime;
        }
        if (!gameObject.GetComponent<AudioSource>().isPlaying && !preparingReplay)
        {
            preparingReplay = true;
            timeUntilReplay = SecondsBeforeReplay;
            Debug.Log(string.Format("Replaying song in {0} seconds.", SecondsBeforeReplay));
        }
        if (preparingReplay && timeUntilReplay <= 0)
        {
            preparingReplay = false;
            Play();
        }
    }

    int FindAssets(string name)
    {
        for (int i = 0; i < Rhythms.Length; i++)
        {
            if (Rhythms[i].name == name && MusicFiles[i].name == name)
            {
                return i;
            } 
        }
        throw new System.ArgumentException("Could not find rhythm and music file: " + name);
    }

    public Song LoadSong(string name, float beatDifficulty, float speedDifficulty)
    {
        int index = FindAssets(name);
        Rhythm r = new Rhythm(Rhythms[index]);
        Song s = new Song();
        s.rhythm = r;
        s.clip = MusicFiles[index];
        s.speedDifficulty = speedDifficulty;
        s.beatDifficulty = beatDifficulty;
        s.name = name;
        return s;
    }

    public void SetSong(Song s)
    {
        gameObject.GetComponent<AudioSource>().Stop();
        BeatSpawner.GetComponent<BeatSpawner>().ClearBeats();
        Debug.Log("Song set to " + s.name);
        song = s;
        gameObject.GetComponent<AudioSource>().clip = song.clip;
        BeatSpawner.GetComponent<BeatSpawner>().SetSong(song);
    }

    public void PlayIfNeeded()
    {
        if (song != lastSongPlayed || !gameObject.GetComponent<AudioSource>().isPlaying)
        {
            Play();
            lastSongPlayed = song;
        }
    }

    private void Play()
    {
        Debug.Log("Now playing: " + song.name);
        gameObject.GetComponent<AudioSource>().Play();
        BeatSpawner.GetComponent<BeatSpawner>().Play();
    }
}
