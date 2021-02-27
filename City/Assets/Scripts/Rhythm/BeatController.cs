using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatController : MonoBehaviour
{
    public static float MaxDistance = 2500;

    public float target;
    public float timeLeft;
    public int channel = 1;
    public int segs;
    public float width;
    public float speed;

    private float startTime;
    private Color color;
    private bool IsActive = true;
    private LineRenderer LR;


    public void Initialize(float targetDistance, float time, float songSpeed, int chan, int segmentsPerCircle, float lineWidth)
    {
        LR = gameObject.AddComponent<LineRenderer>();
        LR.material = BeatSpawner.instance.BeatMaterial;
        target = targetDistance;
        startTime = time;
        timeLeft = time;
        channel = chan;
        speed = songSpeed;
        segs = segmentsPerCircle;
        width = lineWidth;
        SetColor();
        LR = Helpers.DrawCircle(LR, segs, target + DistanceToTarget(), width);
    }

    float DistanceToTarget()
    {
        return (timeLeft / startTime) * (MaxDistance * speed);
    }

    void SetColor()
    {
        switch (channel)
        {
            case 1:
                color = Color.red;
                break;
            case 2:
                color = new Color(0, 0.2f, 1);
                break;
            case 3:
                color = new Color(1, 0.2f, 1);
                break;
        }
        if (timeLeft < 0)
        {
            float badness = Mathf.Abs(timeLeft);
            color = (color * badness);
        }
        LR.material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsActive != BeatSpawner.instance.IsActive)
        {
            Toggle();
        }
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            speed = speed * 0.5f;
            if (Mathf.Abs(timeLeft) > BeatSpawner.instance.OKTimeWindow)
            {
                BeatSpawner.instance.MissBeat();
                DeleteBeat();
            }
        }
        LR = Helpers.DrawCircle(LR, segs, target + DistanceToTarget(), width);
        SetColor();

        // animate LR
        LR.transform.position = new Vector3(LR.transform.position.x, BeatSpawner.instance.transform.position.y /*bounce:*/ /*+ Mathf.Abs(0.5f * (Mathf.Sin(Time.time)))*/, LR.transform.position.z);
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
