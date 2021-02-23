﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSpawner : MonoBehaviour
{
    public static BeatSpawner instance;

    public AudioClip Hit;
    public AudioClip Hit2;
    public GameObject BeatPrefab;
    public GameObject FeedbackPrefab;
    public GameObject Background;
    public GameObject Target;
    public List<GameObject> Beats;

    public bool IsActive = true;
    public Song CurrentSong;
    public Rhythm CurrentRhythm;
    public float ScreenPercentForPerfect;
    public float ScreenPercentForGreat;
    public float ScreenPercentForGood;
    public float ScreenPercentForOK;
    public int MAX_BEATS;
    public Resolution resolution;


    private float PerfectPercent;
    private float GreatPercent;
    private float GoodPercent;
    private float OKPercent;

    private Vector3 targetPos;

    public float BeatOffset; // user setting

    private bool Playing = false;
    private int CurrentBeatIndex = 0;
    private float SongTimer = 0;
    private float NextBeat = 0;
    private int NextChannel;
    private int NextDifficulty;

    void Start()
    {
        instance = this;
        transform.position = new Vector3(Screen.width / 2, Screen.height * 0.1f, 0);
        PerfectPercent = ScreenPercentForPerfect * Screen.width;
        GreatPercent = ScreenPercentForGreat * Screen.width;
        GoodPercent = ScreenPercentForGood * Screen.width;
        OKPercent = ScreenPercentForOK * Screen.width;
        resolution = Screen.currentResolution;
        targetPos = transform.GetChild(1).position;
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

        if (resolution.width != Screen.width /*height info not needed at this time*/)
        {
            PerfectPercent = ScreenPercentForPerfect * Screen.width;
            GreatPercent = ScreenPercentForGreat * Screen.width;
            GoodPercent = ScreenPercentForGood * Screen.width;
            OKPercent = ScreenPercentForOK * Screen.width;
            resolution = Screen.currentResolution;
            targetPos = transform.GetChild(1).position;
        }
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

        if (Beats.Count < MAX_BEATS * CurrentSong.beatDifficulty) //scale max beats with difficulty
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

    void SpawnBeat()
    {
        GameObject NewBeat = Instantiate(BeatPrefab, transform);
        NewBeat.transform.SetParent(gameObject.transform);
        float timeLeft = NextBeat - SongTimer;
        NewBeat.GetComponent<BeatController>().timeLeft = timeLeft;
        NewBeat.transform.position = new Vector3(targetPos.x + -1 * timeLeft * CurrentSong.speedDifficulty * resolution.width, transform.position.y, 0);
        if (!IsActive)
        {
            NewBeat.GetComponent<BeatController>().Toggle();
        }
        if (NextChannel == 2)
        {
            NewBeat.GetComponent<BeatController>().SetColor(Color.blue);
            NewBeat.GetComponent<BeatController>().channel = 2;
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
        Background.SetActive(IsActive);
        Target.SetActive(IsActive);
    }

    void SpawnFeedback(string text, Color color)
    {
        GameObject feedbackText = Instantiate(FeedbackPrefab, transform);
        feedbackText.transform.position = transform.position + new Vector3(0,150,0) + UnityEngine.Random.insideUnitSphere * 150;
        feedbackText.transform.SetParent(transform);
        feedbackText.GetComponent<Text>().text = text;
        feedbackText.GetComponent<Text>().color = color;
        Destroy(feedbackText, 1.0f);
    }

    public void MissBeat()
    {
        Debug.Log("Miss");
        SpawnFeedback("Miss", Color.black);
    }

    void CorrectBeat(string text, Color color)
    {
        BeatController LastBeat = Beats[0].GetComponent<BeatController>();
        Debug.Log(text);
        SpawnFeedback(text, color);
        LastBeat.transform.localScale = new Vector3(1, 1, 1);
        LastBeat.DeleteBeat();
    }

    void ReceiveBeats()
    {
        if (Beats.Count - 1 > 0 && IsActive)
        {
            BeatController LastBeat = Beats[0].GetComponent<BeatController>();
            if (Input.GetMouseButtonDown(0) && LastBeat.channel == 1 
                || Input.GetMouseButtonDown(1) && LastBeat.channel == 2)
            {
                
                float actualPos = LastBeat.transform.position.x;

                if (Mathf.Abs(targetPos.x - (actualPos + BeatOffset)) < PerfectPercent)
                {
                    CorrectBeat("Excellent", Color.yellow);
                    PlayerController.instance.ExcellentBeat();
                }
                else if (Mathf.Abs(targetPos.x - (actualPos + BeatOffset)) < GreatPercent)
                {
                    CorrectBeat("Great", Color.green);
                    PlayerController.instance.GreatBeat();
                }
                else if (Mathf.Abs(targetPos.x - (actualPos + BeatOffset)) < GoodPercent)
                {
                    CorrectBeat("Good", Color.cyan);
                    PlayerController.instance.GoodBeat();
                }
                else if (Mathf.Abs(targetPos.x - (actualPos + BeatOffset)) < OKPercent)
                {
                    CorrectBeat("OK", Color.red);
                    PlayerController.instance.OKBeat();
                }
                else
                {
                    MissBeat();
                }
                if (LastBeat.GetComponent<BeatController>().channel == 2)
                {
                    GetComponent<AudioSource>().clip = Hit2;
                    GetComponent<AudioSource>().Play();
                }
                else
                {
                    GetComponent<AudioSource>().clip = Hit;
                    GetComponent<AudioSource>().Play();
                }
          }
        }
    }
}
