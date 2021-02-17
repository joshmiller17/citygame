using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
             Camera.main.transform.rotation * Vector3.up);

        // only horizontal move
        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        //      Camera.main.transform.rotation * Vector3.up);
        //Vector3 eulerAngles = transform.eulerAngles;
        //eulerAngles.x = 0;
        //transform.eulerAngles = eulerAngles;
    }
}