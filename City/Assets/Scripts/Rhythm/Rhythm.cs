using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhythm
{
    private float[] timings;
    private int[] channels;
    private int[] difficulties;

    public Rhythm(string filePath)
    {
        //Debug.Log("Loading rhythm: " + filePath);
        TextAsset rhythmData = Resources.Load<TextAsset>(filePath);
        List<float> t = new List<float>();
        List<int> c = new List<int>();
        List<int> d = new List<int>();
        string fileData = rhythmData.text;
        string[] lines = fileData.Split('\n');
        foreach (string line in lines)
        {
            string[] data = line.Split();
            if (data.Length < 3)
            {
                break; //EOF
            }
            if (float.Parse(data[0]) < 1.5f)
            {
                continue; // don't introduce sudden beats in first 1.5 sec, let there be a brief intro
            }
            t.Add(float.Parse(data[0]));
            c.Add(int.Parse(data[1]));
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
