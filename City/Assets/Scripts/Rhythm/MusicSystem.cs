using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public float SecondsBeforeReplay;
    public static MusicSystem instance;
    public string SongDir;
    public string RhythmDir;
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

    public Song LoadSong(string name, int beatDifficulty, float speedDifficulty)
    {
        Rhythm r = new Rhythm(RhythmDir + "/" + name);
        Song s = new Song();
        s.rhythm = r;
        //Debug.Log("Loading music " + SongDir + "/" + name);
        s.clip = Resources.Load<AudioClip>(SongDir + "/" + name);
        s.speedDifficulty = speedDifficulty;
        s.beatDifficulty = beatDifficulty;
        s.name = name;
        return s;
    }

    public void SetSong(Song s)
    {
        Debug.Log("Song set to " + s.name);
        song = s;
        gameObject.GetComponent<AudioSource>().clip = song.clip;
        BeatSpawner.GetComponent<BeatSpawner>().SetSong(song);
    }

    public void PlayIfNeeded()
    {
        if (song != lastSongPlayed)
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
