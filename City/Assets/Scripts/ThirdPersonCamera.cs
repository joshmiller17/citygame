using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float speed = 1;
    public GameObject Player;
    public GameObject CamPivot;
    public float rotSpeed = 100;

    private Vector3 offset;
    private float rotDirection = 0;
    private float vertRotDir = 0;
    private int rotLimit = 20;
    private int minDist = 4;
    private int maxDist = 30;


    void Start()
    {
        CamPivot.transform.parent = Player.transform;
        offset = transform.position - CamPivot.transform.position;
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
        CamPivot.transform.position = Player.transform.position;
    }

    void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0.0f)
        {
            if (Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, Player.transform.position) > minDist)
            {
                ChangeZoom();
            }
            else if (Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, Player.transform.position) < maxDist)
            {
                ChangeZoom();
            }
        }
    }

    void HandleCameraPivot()
    {
        if (Input.GetMouseButton(1))
        { // right click
            rotDirection += Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
            //rotDirection = BindToRotLimit(rotDirection); //Don't limit x rotation
            vertRotDir += Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed;
            vertRotDir = BindToRotLimit(vertRotDir);

            CamPivot.transform.rotation = Quaternion.Euler(-1 * vertRotDir, rotDirection, 0);
        }
    }

    float BindToRotLimit(float rotDir) {
        if (rotDir > rotLimit)
        {
            rotDir = rotLimit;
        }
        if (rotDir < -1 * rotLimit)
        {
            rotDir = -1 * rotLimit;
        }
        return rotDir;
    }

    void ChangeZoom()
    {
        offset = transform.position - CamPivot.transform.position;
        offset *= (10 - Input.mouseScrollDelta.y) / 10;
        transform.position = CamPivot.transform.position + offset;

    }

}
