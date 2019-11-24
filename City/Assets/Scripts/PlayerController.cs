using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Text DebugInfo;
    public GameObject Camera;
    public GameObject MusicSystem;
    public CharacterController controller;
    public Transform pivot;
    public float rotateSpeed;

    //TODO link to character model

    public float SongFadeTime;
    public float CoyoteTime;
    public float jumpForce;
    public float jumpDuration;

    public float moveSpeed;
    public float gravityScale;
    public float maxSpeedMultiplier;

    private float jumpAmountLeft;
    private float speedMultiplier;

    private Vector3 moveDirection = new Vector3(0,0,0);
    private float TimeSinceLastGround = 0;
    private bool isJumping = false;
    private int jumpBuffer = 0; //TODO add a jump buffer for inputting jump before hitting ground
    private AudioSource aud;
    private bool MusicOn = true;

    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        speedMultiplier = 1;

        controller = GetComponent<CharacterController>();
        aud = MusicSystem.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown("c")) //debug cheat
        {
            speedMultiplier = maxSpeedMultiplier;
        }

      if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)){
            BeatSpawner.instance.Toggle();
            MusicOn = !MusicOn;
        }
      FadeSong(MusicOn);

        lastPosition = transform.position;
        Move();
    }

    void LateUpdate()
    {
        DebugInfo.text = "Speed multiplier: " + speedMultiplier.ToString();
        DebugInfo.text += "\nCurrent speed: " + (Vector3.Distance(transform.position, lastPosition)).ToString();
    }

    bool IsMovingFast()
    {
        return (Vector3.Distance(transform.position, lastPosition)) > 0.1f;
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float yStore = moveDirection.y;

        moveDirection = (transform.forward * v) + (transform.right * h);
        moveDirection = moveDirection.normalized * moveSpeed * speedMultiplier;
        moveDirection.y = yStore;

        if (controller.isGrounded)
        {
            TimeSinceLastGround = 0;
            moveDirection.y = 0;
        }
        else
        {
            TimeSinceLastGround += Time.deltaTime;
            jumpAmountLeft = Mathf.Max(jumpAmountLeft - Time.deltaTime, 0);
        }


        if (TimeSinceLastGround < CoyoteTime && Input.GetButton("Jump"))
        {
            if (!isJumping)
            {
                jumpAmountLeft = jumpDuration + ((speedMultiplier - 1) * 0.1f);
            }
            isJumping = true;
        }
        if (Input.GetButtonUp("Jump") || jumpAmountLeft <= 0)
        {
            isJumping = false;
        }

        if (isJumping)
        {
            moveDirection.y = jumpForce + Mathf.Pow(speedMultiplier, 1.01f);
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

        if (!IsMovingFast() || !MusicOn) //slow down if we're not moving and listening to music
        {
            speedMultiplier = Mathf.Max(speedMultiplier * 0.999f, 1);
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

    public void ExcellentBeat()
    {
        speedMultiplier = Mathf.Min(speedMultiplier * 1.02f, maxSpeedMultiplier);
    }

    public void GoodBeat()
    {
        speedMultiplier = Mathf.Min(speedMultiplier * 1.007f, maxSpeedMultiplier);
    }

    public void OKBeat()
    {
        speedMultiplier = Mathf.Min(speedMultiplier * 1.002f, maxSpeedMultiplier);
    }

    public void MissBeat()
    {
        speedMultiplier = Mathf.Max(speedMultiplier * 0.75f, 1);
    }
}
