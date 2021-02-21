using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//state of familiarity
public class Familiarity
{
    private int lastDateOfInteraction = -1;
    private float familiarity = 0;
    private int lastDateUpdated = 0;
    private int minimumFamiliarity = 0;

    void UpdateFamiliarity()
    {
        int daysMissed = PlayerController.instance.todaysDate - lastDateOfInteraction;
        familiarity = Mathf.Max(minimumFamiliarity, familiarity - daysMissed * 0.25f);
        lastDateUpdated = PlayerController.instance.todaysDate;
    }

    void UpdateMinFamiliarity()
    {
        if (familiarity >= 4 && minimumFamiliarity < 1)
        {
            minimumFamiliarity = 1;
        }
        if (familiarity >= 5 && minimumFamiliarity < 2)
        {
            minimumFamiliarity = 2;
        }
        if (familiarity >= 6 && minimumFamiliarity < 3)
        {
            minimumFamiliarity = 3;
        }
    }

    public bool InteractedToday()
    {
        return lastDateOfInteraction == PlayerController.instance.todaysDate;
    }

    public bool Interact()
    {
        if (!InteractedToday())
        {
            familiarity += 1;
            lastDateOfInteraction = PlayerController.instance.todaysDate;
            UpdateFamiliarity();
            UpdateMinFamiliarity();
            return true;
        }
        return false;
    }

    public float GetFamiliarity()
    {
        UpdateFamiliarity();
        return familiarity;
    }
}
