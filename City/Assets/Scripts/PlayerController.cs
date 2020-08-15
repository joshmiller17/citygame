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
    public GameObject dialogueBox;
    public GameObject talkerBox;
    public GameObject speechBox;
    public GameObject thoughtBox;
    public GameObject thoughtText;
    public GameObject moneyText;
    public GameObject interactionInfo;
    public float rotateSpeed;
    public int money = 0;
    public float energy = 100f;
    public float health = 100f;
    public float energyLostPerSecond = 0.1f;

    //TODO link to character model

    public float SongFadeTime;
    public float CoyoteTime;
    public float jumpForce;
    public float jumpDuration;

    public float moveSpeed;
    public float gravityScale;
    public float maxSpeedMultiplier;
    public float maxTalkingRange;

    private float jumpAmountLeft;
    private float speedMultiplier;

    private Vector3 moveDirection = new Vector3(0,0,0);
    private float TimeSinceLastGround = 0;
    private bool isJumping = false;
    private int jumpBuffer = 0; //TODO add a jump buffer for inputting jump before hitting ground
    private AudioSource aud;
    private bool MusicOn = true;
    private GameObject whosTalking = null;
    private float thoughtTimer = 0;
    private bool needToWork = true;
    private Shop availableShop = null;

    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        speedMultiplier = 1;

        controller = GetComponent<CharacterController>();
        aud = MusicSystem.GetComponent<AudioSource>();

        haveThought("I should go to work");
    }

    // Update is called once per frame
    void Update()
    {
        if (thoughtTimer <= 0)
        {
            thoughtBox.SetActive(false);
        }
        else
        {
            thoughtTimer -= Time.deltaTime;
        }

        moneyText.GetComponent<Text>().text = string.Format("Money: ${0}\nEnergy: {1}\nHealth: {2}\nTo Do: {3}", money, energy.ToString("F1"), health.ToString("F1"), needToWork ? "work" : "sleep");
        energy -= Time.deltaTime * energyLostPerSecond;
        if (energy < 0)
        {
            //take negative energy as damage and speed penalty
            health += energy;
            energy = 0;
            speedMultiplier *= 0.998f; //can go below 1?
        }

        GameObject Talker = GetNearestNPCWithinRange(maxTalkingRange);
        if (Talker != null && (Input.GetKeyDown("z") || Talker.GetComponent<NPC>().talksFirst))
        {
            whosTalking = Talker;
            talkerBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetName();
            speechBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetSpeech();
            dialogueBox.SetActive(true);
            //Dialogue.instance.newDialogue(name, speech);

        }
        else if (Talker != whosTalking)
        {
            dialogueBox.SetActive(false);
        }


        if (Input.GetKeyDown("c")) //debug cheat
        {
            speedMultiplier = maxSpeedMultiplier;
        }

      if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)){
            BeatSpawner.instance.Toggle();
            MusicOn = !MusicOn;
        }
      FadeSong(MusicOn);

        if (Input.GetKeyDown("e") && availableShop != null)
        {
            availableShop.Show();
            return; //don't move
        }

        lastPosition = transform.position;
        Move();
    }


    void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider);
        if (collider.gameObject.name == "WorkTrigger")
        {
            if (needToWork)
            {
                needToWork = false;
                money += 50;
                haveThought("I should sleep");
            }
            else
            {
                haveThought("I already worked today, I should sleep");
            }
        }

        if (collider.gameObject.name == "HomeTrigger")
        {
            if (!needToWork)
            {
                needToWork = true;
                energy = 100;
                haveThought("I should work");
            }
            else
            {
                haveThought("I already slept today, I should work");
            }
        }

        if (collider.gameObject.name == "ShopTrigger")
        {
            availableShop = collider.gameObject.GetComponentInParent<Shop>();
            interactionInfo.GetComponent<Text>().text = "Press E to Shop";
            interactionInfo.SetActive(true);
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "ShopTrigger")
        {
            availableShop = null;
            interactionInfo.SetActive(false);
        }
    }

        void haveThought(string thought, float lifespan = 3f)
    {
        thoughtText.GetComponent<Text>().text = thought;
        thoughtBox.SetActive(true);
        thoughtTimer = lifespan;
    }

    void LateUpdate()
    {
        DebugInfo.text = "Speed multiplier: " + speedMultiplier.ToString();
        DebugInfo.text += "\nCurrent speed: " + (Vector3.Distance(transform.position, lastPosition)).ToString();
    }

    GameObject GetNearestNPCWithinRange(float maxRangeAllowed)
    {
        GameObject[] npcs;
        npcs = GameObject.FindGameObjectsWithTag("NPC");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject npc in npcs)
        {
            Vector3 diff = npc.transform.position - position;
            float curDistance = diff.sqrMagnitude;
           // Debug.Log("Distance to NPC is " + curDistance);
            if (curDistance < distance && curDistance < maxRangeAllowed)
            {
                closest = npc;
                distance = curDistance;
            }
        }
        return closest;
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
