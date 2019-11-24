using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSpawner : MonoBehaviour
{
    public static BeatSpawner instance;

    public AudioSource Hit;
    public GameObject BeatPrefab;
    public GameObject FeedbackPrefab;
    public List<GameObject> Beats;

    public bool IsActive = true;
    public float BeatInterval;
    public float BeatSpeed; //min 1 (slowest)
    public float ScreenPercentForPerfect;
    public float ScreenPercentForGood;

    public float BeatAdjustment;

    private float ToNextBeat;

    void Start()
    {
        instance = this;

        ToNextBeat = BeatInterval;
        transform.position = new Vector3(Screen.width / 2, Screen.height * 0.1f, 0);

        ScreenPercentForPerfect *= Screen.width; //fixme if screen width changes, need to update these numbers
        ScreenPercentForGood *= Screen.width; 
    }

    void Update()
    {
        TriggerBeatSpawnIfNeeded();
        ReceiveBeats();
    }

    void TriggerBeatSpawnIfNeeded()
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
        NewBeat.transform.position = transform.position - new Vector3(BeatSpeed * 0.5f * Screen.width, 0, 0);
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

    public void MissBeat()
    {
        GameObject feedbackText = Instantiate(FeedbackPrefab, transform);
        feedbackText.transform.position = transform.position + Random.insideUnitSphere * 100;
        feedbackText.transform.SetParent(gameObject.transform);
        feedbackText.GetComponent<Text>().text = "Miss";
        Object.Destroy(feedbackText, 1.0f);
        Debug.Log("Miss");
        PlayerController.instance.MissBeat();
    }

    void ReceiveBeats()
    {
        if (Beats.Count - 1 > 0 && IsActive) {
            if (Input.GetMouseButtonDown(0))
            {
                BeatController LastBeat = Beats[0].GetComponent<BeatController>();
                //Debug.Log("Last beat has position " + LastBeat.transform.position.x);
                float actualPos = LastBeat.transform.position.x;
                float targetPos = transform.position.x; 

                if (Mathf.Abs(targetPos - (actualPos + BeatAdjustment)) < ScreenPercentForPerfect)
                {
                    Debug.Log("Excellent");

                    GameObject feedbackText = Instantiate(FeedbackPrefab, transform);
                    feedbackText.transform.position = transform.position + Random.insideUnitSphere * 100;
                    feedbackText.transform.SetParent(gameObject.transform);
                    feedbackText.GetComponent<Text>().text = "Excellent";
                    Object.Destroy(feedbackText, 1.0f);
                    LastBeat.transform.localScale = new Vector3(1, 1, 1);
                    LastBeat.DeleteBeat();
                    PlayerController.instance.ExcellentBeat();
                }
                else if (Mathf.Abs(targetPos - (actualPos + BeatAdjustment)) < ScreenPercentForGood / 2)
                {
                    Debug.Log("Good");
                    GameObject feedbackText = Instantiate(FeedbackPrefab, transform);
                    feedbackText.transform.position = transform.position + Random.insideUnitSphere * 100;
                    feedbackText.transform.SetParent(gameObject.transform);
                    feedbackText.GetComponent<Text>().text = "Good";
                    Object.Destroy(feedbackText, 1.0f);
                    LastBeat.transform.localScale = new Vector3(1, 1, 1);
                    LastBeat.DeleteBeat();
                    PlayerController.instance.GoodBeat();
                }
                else if (Mathf.Abs(targetPos - (actualPos + BeatAdjustment)) < ScreenPercentForGood)
                {
                    Debug.Log("OK");
                    GameObject feedbackText = Instantiate(FeedbackPrefab, transform);
                    feedbackText.transform.position = transform.position + Random.insideUnitSphere * 100;
                    feedbackText.transform.SetParent(gameObject.transform);
                    feedbackText.GetComponent<Text>().text = "OK";
                    Object.Destroy(feedbackText, 1.0f);
                    LastBeat.transform.localScale = new Vector3(1, 1, 1);
                    LastBeat.DeleteBeat();
                    PlayerController.instance.OKBeat();
                }
                else
                {
                    MissBeat();
                }
                Hit.Play();
            }
        }
    }
}
