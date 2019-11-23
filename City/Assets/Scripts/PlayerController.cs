using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject BeatSpawner;
    public GameObject Camera;
    public GameObject MusicSystem;
    public CharacterController controller;
    public Transform pivot;
    public float rotateSpeed;

    //TODO link to character model

    public float SongFadeTime = 0.5f;
    public float CoyoteTime = 0.3f;
    public float jumpForce;
    public float jumpDuration;

    public float moveSpeed;
    public float gravityScale;

    private Vector3 moveDirection = new Vector3(0,0,0);
    private float TimeSinceLastGround = 0;
    private bool isJumping = false;
    private int jumpBuffer = 0; //TODO add a jump buffer for inputting jump before hitting ground
    private AudioSource aud;
    private bool MusicOn = true;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        aud = MusicSystem.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)){
            BeatSpawner.GetComponent<BeatSpawner>().Toggle();
            MusicOn = !MusicOn;
        }
      FadeSong(MusicOn);

        Move();
    }

    void LateUpdate()
    {
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float yStore = moveDirection.y;

        moveDirection = (transform.forward * v) + (transform.right * h);
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (controller.isGrounded)
        {
            TimeSinceLastGround = 0;
            moveDirection.y = 0;
        }
        else
        {
            TimeSinceLastGround += Time.deltaTime;
        }


        if (TimeSinceLastGround < CoyoteTime && Input.GetButton("Jump"))
        {
            isJumping = true;
        }
        if (Input.GetButtonUp("Jump") || TimeSinceLastGround > CoyoteTime)
        {
            isJumping = false;
        }

        if (isJumping)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = moveDirection.y + (gravityScale * Physics.gravity.y);
        }

        controller.Move(moveDirection * Time.deltaTime);

        if (h != 0 || v != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));

            //FIXME this causes jitter when walking backwards
            //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime); //TODO rotate the player model, not the transform
        }

    }

    void FadeSong(bool MusicOn)
    {
        if (MusicOn && aud.volume < 1) // TODO change 1 to a variable set by player if they want to adjust music settings
        {
            aud.volume += Time.deltaTime / SongFadeTime;
        }
        else if (!MusicOn && aud.volume > 0)
        {
            aud.volume -= Time.deltaTime / SongFadeTime;
        }
    }
}
