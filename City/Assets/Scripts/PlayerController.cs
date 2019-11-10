using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject BeatSpawner;
    public GameObject CamPivot;
    public GameObject MusicSystem;
    public float moveSpeed = 200;
    public float SongFadeTime = 0.5f;
    public float CoyoteTime = 0.3f;
    public float JumpForce;
    public float JumpDuration;

    private float TimeSinceLastGround = 0;
    private float JumpLeft = 0;
    private AudioSource audio;
    private bool MusicOn = true;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = MusicSystem.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)){
            BeatSpawner.GetComponent<BeatSpawner>().Toggle();
            MusicOn = !MusicOn;
        }
      FadeSong(MusicOn);
    }

    // For physics code, before physics calculatinos
    void FixedUpdate()
    {
        Move();

        bool isGrounded = Physics.CheckCapsule(GetComponent<Collider>().bounds.center, new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.min.y - 0.1f, GetComponent<Collider>().bounds.center.z), 0.18f);
        if (isGrounded)
        {
            TimeSinceLastGround = 0;
        }
        else
        {
            TimeSinceLastGround += Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") && TimeSinceLastGround < CoyoteTime)
        {
            JumpLeft = JumpDuration;
        }
        else {
            JumpLeft = Mathf.Max(JumpLeft - Time.deltaTime, 0);
        }
        DoJump(JumpLeft);
    }

    void DoJump(float amount)
    {
        if (amount == 0)
        {
            return;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + JumpForce, transform.position.z);
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

    void FadeSong(bool MusicOn)
    {
        if (MusicOn && audio.volume < 1) // TODO change 1 to a variable set by player if they want to adjust music settings
        {
            audio.volume += Time.deltaTime / SongFadeTime;
        }
        else if (!MusicOn && audio.volume > 0)
        {
            audio.volume -= Time.deltaTime / SongFadeTime;
        }
    }
}
