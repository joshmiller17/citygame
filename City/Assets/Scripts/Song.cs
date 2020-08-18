using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song
{
    public AudioClip clip;
    public float[] rhythm;

    public void SetRhythm(float[] r)
    {
        rhythm = r;
    }
}