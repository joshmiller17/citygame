using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnvObj
{
    public string[] fam1Speeches;
    public string[] fam2Speeches;
    public string[] fam3Speeches;
    public string characterName;
    public bool talksFirst;
    public Vector3 origin;
    [Range(0, 10)] public float speed;
    [Tooltip("Length of travel in single direction")]
    [Range(0, 10)] public float decisiveness;
    [Tooltip("Chance of moving in new direction")]
    [Range(0, 100)] public float jittery;
    [Tooltip("Range of motion")]
    [Range(0, 100)] public float wanderlust;

    private float continuing;
    private Vector3 direction = new Vector3(0, 0, 0);

    public override void Start()
    {
        base.Start();
        origin = transform.position;
    }

    private void Update()
    {
        continuing -= Time.deltaTime;
        if (continuing <= 0)
        {
            if (Random.value * 100 < jittery)
            {
                direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            }
            else
            {
                direction = new Vector3(0, 0, 0);
            }
            continuing += Random.Range(0f, decisiveness);
        }

        gameObject.transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, origin) > wanderlust)
        {
            direction = (origin - transform.position).normalized;
        }
    }

    public string GetSpeech()
    {
        if (fam.GetFamiliarity() <= 1)
        {
            return fam1Speeches[Random.Range(0, fam1Speeches.Length)];
        }
        else if (fam.GetFamiliarity() <= 3)
        {
            return fam2Speeches[Random.Range(0, fam1Speeches.Length)];
        }
       else
        {
            return fam3Speeches[Random.Range(0, fam1Speeches.Length)];
        }
    }

    public string GetName()
    {
        if (fam.GetFamiliarity() <= 1)
        {
            return "???";
        }
        return characterName;
    }

    void StopMoving()
    {
        direction = new Vector3(0, 0, 0);
        continuing = 10;
    }

    public override void Interact()
    {
        base.Interact();
        StopMoving();
    }
}
