using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSpawner : MonoBehaviour
{
    public static BeatSpawner instance;

    [Header("Links")]
    public GameObject FeedbackPrefab;
    public Material BeatMaterial;
    public GameObject BeatFeedbackSpawner;
    public AudioClip[] ExcellentSounds;
    public AudioClip[] GreatSounds;
    public AudioClip[] GoodSounds;
    public AudioClip[] OKSounds;
    public AudioClip[] MissSounds;

    [Space(10)]
    [Header("Settings")]
    public float TargetDistance = 5;
    public float ExcellentTimeWindow = .05f;
    public float GreatTimeWindow = .1f;
    public float GoodTimeWindow = .2f;
    public float OKTimeWindow = .4f;
    public int MaxBeats = 6;
    public float BeatOffset; // user setting
    public int segmentsPerCircle = 15;
    public float lineWidth = 0.1f;

    [HideInInspector] public bool IsActive = true;
    [HideInInspector] public Song CurrentSong;
    [HideInInspector] public Rhythm CurrentRhythm;
    [HideInInspector] public List<GameObject> Beats;

    private bool Playing = false;
    private int CurrentBeatIndex = 0;
    private float SongTimer = 0;
    private float NextBeat = 0;
    private int NextChannel;
    private int NextDifficulty;
    private LineRenderer LR;

    void Start()
    {
        instance = this;
        LR = gameObject.AddComponent<LineRenderer>();
        LR = Helpers.DrawCircle(LR, segmentsPerCircle, TargetDistance, lineWidth);
        LR.material = BeatMaterial;
        LR.material.color = Color.white;
    }

    void Update()
    {
        if (!MusicSystem.instance.gameObject.GetComponent<AudioSource>().isPlaying)
        {
            Playing = false;
        }
        if (!Playing) return;
        TriggerBeatSpawnIfNeeded();
        ReceiveBeats();
        LR.transform.Rotate(new Vector3(0, 1, 0));
    }

    public void SetSong(Song s)
    {
        CurrentSong = s;
        CurrentRhythm = s.rhythm;
    }

    public void Play()
    {
        SongTimer = 0;
        CurrentBeatIndex = 0;
        Tuple<float, int, int> next = CurrentRhythm.GetBeat(CurrentBeatIndex);
        NextBeat = next.Item1;
        NextChannel = next.Item2;
        NextDifficulty = next.Item3;
        Playing = true;
    }

    void TriggerBeatSpawnIfNeeded()
    {
        SongTimer += Time.deltaTime;

        if (Beats.Count < MaxBeats * CurrentSong.beatDifficulty) //scale max beats with difficulty
        {
            CurrentBeatIndex += 1;
            Tuple<float, int, int> next = CurrentRhythm.GetBeat(CurrentBeatIndex);
            if (next != null)
            {
                NextBeat = next.Item1;
                NextChannel = next.Item2;
                NextDifficulty = next.Item3;
                if (NextDifficulty <= CurrentSong.beatDifficulty)
                {
                    SpawnBeat();
                }
            }
            else
            {
                //CurrentBeatIndex = 0;
                //TODO reset the song timer when the song ends, then set CurrentBeatIndex to 0
            }
        }
    }

    public void ClearBeats()
    {
        foreach (GameObject beat in Beats)
        {
            Destroy(beat);
        }
        Beats.Clear();
    }

    void SpawnBeat()
    {
        GameObject NewBeat = Instantiate(new GameObject("Beat"), transform);
        BeatController bc = NewBeat.AddComponent<BeatController>();
        NewBeat.transform.SetParent(gameObject.transform);
        bc.Initialize(TargetDistance, NextBeat - SongTimer, CurrentSong.speedDifficulty, NextChannel, segmentsPerCircle, lineWidth);
        Beats.Add(NewBeat);
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        foreach (GameObject beat in Beats)
        {
            beat.GetComponent<BeatController>().Toggle();
        }

        if (!IsActive)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void SpawnFeedback(string text, Color color)
    {
        GameObject feedbackText = Instantiate(FeedbackPrefab, BeatFeedbackSpawner.transform);
        feedbackText.transform.position = BeatFeedbackSpawner.transform.position + new Vector3(0,0,0) + UnityEngine.Random.insideUnitSphere * 100;
        feedbackText.transform.SetParent(BeatFeedbackSpawner.transform);
        feedbackText.GetComponent<Text>().text = text;
        feedbackText.GetComponent<Text>().color = color;
        Destroy(feedbackText, 1.0f);
    }

    public void MissBeat()
    {
        Debug.Log("Miss");
        SpawnFeedback("Miss", Color.black);
        PlayerController.instance.MissBeat();
    }

    void CorrectBeat(string text, Color color)
    {
        BeatController LastBeat = Beats[0].GetComponent<BeatController>();
        Debug.Log(text);
        SpawnFeedback(text, color);
        LastBeat.DeleteBeat();
        
    }

    void PlayRandomSound(AudioClip[] sounds)
    {
        GetComponent<AudioSource>().clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        GetComponent<AudioSource>().Play();
    }

    void ReceiveBeats()
    {
        if (Beats.Count - 1 > 0 && IsActive)
        {
            BeatController LastBeat = Beats[0].GetComponent<BeatController>();
            float timeDistance = Math.Abs(LastBeat.timeLeft);

            if (Input.GetMouseButtonDown(0) && LastBeat.channel == 1 
                || Input.GetMouseButtonDown(1) && LastBeat.channel == 2
                || (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1) && LastBeat.channel == 3)
                )
            {

                if (timeDistance < ExcellentTimeWindow)
                {
                    CorrectBeat("Excellent", Color.yellow);
                    PlayerController.instance.ExcellentBeat();
                    PlayRandomSound(ExcellentSounds);
                }
                else if (timeDistance < GreatTimeWindow)
                {
                    CorrectBeat("Great", Color.green);
                    PlayerController.instance.GreatBeat();
                    PlayRandomSound(GreatSounds);
                }
                else if (timeDistance < GoodTimeWindow)
                {
                    CorrectBeat("Good", Color.cyan);
                    PlayerController.instance.GoodBeat();
                    PlayRandomSound(GoodSounds);
                }
                else if (timeDistance < OKTimeWindow)
                {
                    CorrectBeat("OK", Color.red);
                    PlayerController.instance.OKBeat();
                    PlayRandomSound(OKSounds);
                }
                else
                {
                    MissBeat();
                    PlayRandomSound(MissSounds);
                }
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                MissBeat();
                PlayRandomSound(MissSounds);
            }
        }
    }
}
