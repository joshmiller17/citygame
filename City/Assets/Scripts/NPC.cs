using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string speech;
    public string characterName;
    public bool talksFirst;
    public Familiarity fam = new Familiarity();
    public Vector3 origin;
    public float speed;
    public float decisiveness;
    public float jittery;
    public float wanderlust;

    private float continuing;
    private Vector3 direction = new Vector3(0, 0, 0);

    private void Start()
    {
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
        return speech;
    }

    public string GetName()
    {
        return characterName;
    }

    void StopMoving()
    {
        direction = new Vector3(0, 0, 0);
        continuing = 10;
    }

    public void Interact()
    {
        fam.Interact();
        StopMoving();
    }
}
