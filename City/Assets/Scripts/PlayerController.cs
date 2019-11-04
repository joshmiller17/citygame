using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject CamPivot;
    private Rigidbody rb;
    public float moveSpeed = 200;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    // For physics code, before physics calculatinos
    void FixedUpdate()
    {
        Move();
    }

    void LateUpdate()
    {
        Rotate();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime;

        Vector3 movement = (transform.forward) * moveVertical + (transform.right) * moveHorizontal;
        //movement = movement.normalized;
        transform.position += movement * moveSpeed;
    }

    void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            transform.rotation = CamPivot.transform.rotation;
        }
    }
}
