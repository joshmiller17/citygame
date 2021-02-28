using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhythm
{
    private static float MIN_DIFFERENCE = 0.017f; // anything within 17ms of each other, consider the same beat 
    private static float WAIT_TO_START = 1.5f;  // don't introduce sudden beats in first 1.5 sec, let there be a brief intro

    private float[] timings;
    private int[] channels;
    private int[] difficulties;

    public Rhythm(TextAsset rhythmData)
    {
        List<float> t = new List<float>();
        List<int> c = new List<int>();
        List<int> d = new List<int>();
        string fileData = rhythmData.text;
        string[] lines = fileData.Split('\n');
        float previousTiming = -999f;

        foreach (string line in lines)
        {
            string[] data = line.Split();
            if (data.Length < 3)
            {
                break; //EOF
            }
            if (float.Parse(data[0]) < WAIT_TO_START)
            {
                continue;
            }

            float time = float.Parse(data[0]);
            int chan = int.Parse(data[1]);

            if (time - previousTiming < MIN_DIFFERENCE)
            {
                time = previousTiming;
                chan = 3;
            }

            previousTiming = time;
            t.Add(time);
            c.Add(chan);
            d.Add(int.Parse(data[2]));
        }
        timings = t.ToArray();
        channels = c.ToArray();
        difficulties = d.ToArray();
    }

    // better way to implement this for de-coupling?
    public Tuple<float, int, int> GetBeat(int index)
    {
        if (index < 0 || index >= timings.Length)
        {
            return null;
        }
        return new Tuple<float, int, int>(timings[index], channels[index], difficulties[index]);
    }
}
