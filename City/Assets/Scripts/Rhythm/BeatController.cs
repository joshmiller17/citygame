using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatController : MonoBehaviour
{
    public float timeLeft;
    public int channel = 1;
    private bool IsActive = true;

    //public bool IsMirror = false;
    //private GameObject Mirror;

    void Start()
    {
        //if (!IsMirror)
        //{
        //    Mirror = Instantiate(BeatSpawner.instance.BeatPrefab, transform);
        //    Mirror.transform.SetParent(gameObject.transform);
        //    Mirror.GetComponent<BeatController>().IsMirror = true;
        //}
    }

    public void SetColor(Color c)
    {
        GetComponent<Image>().color = c;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive != BeatSpawner.instance.IsActive)
        {
            Toggle();
        }
        timeLeft -= Time.deltaTime;
        //if (!IsMirror)
        //{
        float target = BeatSpawner.instance.transform.GetChild(1).position.x;
        float speed = BeatSpawner.instance.CurrentSong.speedDifficulty;
        float width = BeatSpawner.instance.resolution.width;
        transform.position = new Vector3(target + -1 * timeLeft * speed * width, BeatSpawner.instance.transform.position.y, 0);

        if (transform.position.x > target)
        {
            //    transform.localScale = new Vector3(0, 0, 0); //invisible but still active
            SetColor(Color.black); //test
        }


        if (transform.position.x > target + width * BeatSpawner.instance.ScreenPercentForGreat)
        {
            if (IsActive)
            {
                BeatSpawner.instance.MissBeat();
            }
            DeleteBeat();
        }

            //also move mirror
        //    Mirror.transform.position = new Vector3(target + - 1 * (transform.position.x - target), transform.position.y, transform.position.z);
        //    Mirror.GetComponent<Image>().color = GetComponent<Image>().color;
        //}
        //else
        //{
        //    //Mirror
        //}
    }

    public void DeleteBeat()
    {
        BeatSpawner.instance.Beats.Remove(gameObject);
        Destroy(gameObject);
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        if (!IsActive)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (IsActive && timeLeft < 1) { // give at least one second warning
            IsActive = false;
        }
    }
}
