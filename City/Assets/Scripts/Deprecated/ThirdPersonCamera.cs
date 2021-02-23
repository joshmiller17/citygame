// Author: Josh Aaron Miller
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Transform pivot;
    public Vector3 offset;
    public bool useOffsetValues;
    public float rotateSpeed;
    public bool YaxisInvert;

    private int minDist = 4;
    private int maxDist = 30;


    void Start()
    {

        if (!useOffsetValues)
        {
            offset = target.position - transform.position;
        }

        pivot.transform.position = target.transform.position;
        pivot.transform.parent = null;
        // to zoom out, multiply; to zoom in, divide

        //Cursor.lockState = CursorLockMode.Locked; //hide cursor; TODO fix for things like menus
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        HandleCameraZoom();
        HandleCameraPivot();
        pivot.transform.position = target.transform.position;
    }

    void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0.0f)
        {
            if (Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, target.transform.position) > minDist)
            {
                ChangeZoom(Input.mouseScrollDelta.y);
            }
            else if (Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, target.transform.position) < maxDist)
            {
                ChangeZoom(Input.mouseScrollDelta.y);
            }
        }
    }

    void HandleCameraPivot()
    {
        if (Input.GetMouseButton(1)) // right click
        {
            float horizontal = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            pivot.Rotate(0, horizontal, 0);

            int invert = YaxisInvert ? -1 : 1;
            float vert = invert * Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

            // this code is no longer relevant because of how billboarding is happening
            //if ((pivot.rotation.x < 0.3f && vert > 0) || (pivot.rotation.x > -0.3f && vert < 0)) //max/min view angle
            //{
            //    pivot.Rotate(vert, 0, 0);
            //}

            Cursor.lockState = CursorLockMode.Locked; //TODO, replace cursor to sprite so it can be hidden smartly later
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        float Yangle = pivot.eulerAngles.y;
        float Xangle = pivot.eulerAngles.x;
        Quaternion rot = Quaternion.Euler(Xangle, Yangle, 0);
        transform.position = target.position - (rot * offset);

        if (transform.position.y < target.position.y)
        {
            transform.position = new Vector3(transform.position.x, target.position.y - 0.5f, transform.position.z);
        }

        transform.LookAt(target);

    }

    public void ChangeZoom(float amount)
    {
        offset *= (10 - amount) / 10;
    }



}
