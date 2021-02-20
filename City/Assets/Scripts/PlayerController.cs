using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Rhythm Multipliers")]
    public float ExcellentBeatMult;
    public float GreatBeatMult;
    public float GoodBeatMult;
    public float OKBeatMult;
    public float MissBeatMult;

    [Space(10)]
    [Header("Global Settings")]

    [Range(0f, 1f)]
    public float PercentLightInDay;
    public float SongFadeTime;
    public bool MusicOn = true;

    [Space(10)]
    [Header("Global Stats")]
    public int todaysDate = 0;
    public float timeOfDay;
    public int secondsInDay;

    [Space(10)]
    [Header("Player Settings")]
    public float CoyoteTime;
    public float jumpForce;
    public float jumpDuration;
    public float energyLostPerSecond;
    public float foodLostPerEnergy;
    public float rotateSpeed;
    public float workPerSecond;
    public float moveSpeed;
    public float gravityScale;
    public float maxSpeedMultiplier;
    public float maxTalkingRange;
    public bool YaxisInvert;

    [Space(10)]
    [Header("Player Stats")]
    public int MaxHP;
    public int MaxEnergy;
    public int MaxFood;
    public float money;
    public float energy;
    public float health;
    public float food;
    public float jumpAmountLeft;
    public float speedMultiplier;
    public float speedBoost = 0;
    public float boostDuration = 0;
    public float jumpMultiplier = 1;


    [Header("GameObj Links")]
    public Material BlendedSkybox;
    public Material[] Skyboxes;
    public Text DebugInfo;
    public GameObject TPCamera;
    public GameObject MusicSysObj;
    public CharacterController controller;
    public Transform pivot;
    public GameObject dialogueBox;
    public GameObject talkerBox;
    public GameObject speechBox;
    public GameObject portrait;
    public GameObject thoughtBox;
    public GameObject thoughtText;
    public GameObject moneyText;
    public GameObject interactionInfo;
    public MusicSystem MusicSys;
    // TODO character model
    
    private Vector3 moveDirection = new Vector3(0, 0, 0);
    private float TimeSinceLastGround = 0;
    private bool isJumping = false;
    //private int jumpBuffer = 0; //TODO add a jump buffer for inputting jump before hitting ground

    private AudioSource aud;
    private NameGenerator NameGen = new NameGenerator();
    private GameObject whosTalking = null;
    private float thoughtTimer = 0;
    private bool hungry = false;
    private bool needToWork = true;
    private bool working = false;
    private Shop availableShop = null;
    private Vector3 lastPosition;

    void Start()
    {
        instance = this;
        speedMultiplier = 1;
        MusicSys = MusicSysObj.GetComponent<MusicSystem>();

        controller = GetComponent<CharacterController>();
        aud = MusicSys.GetComponent<AudioSource>();

        haveThought("I should go to work");

        //TEST
        Song s = MusicSys.LoadSong("AcousticRock", 2, .08f);
        MusicSys.SetSong(s);
        MusicSys.PlayIfNeeded();

        NameGen.Init();
    }

    bool isNight()
    {
        return timeOfDay >= secondsInDay * PercentLightInDay;
    }

    void updateTimeOfDay()
    {
        timeOfDay += Time.deltaTime;
        boostDuration = Mathf.Max(boostDuration - Time.deltaTime, 0);
        if (timeOfDay > secondsInDay)
        {
            timeOfDay -= secondsInDay;
            todaysDate += 1;
        }

        float distanceFromNoon = Mathf.Abs(timeOfDay - (secondsInDay / 2));
        BlendedSkybox.SetFloat("_Blend", (1 - (distanceFromNoon / (secondsInDay / 2))));
        //RenderSettings.skybox = Skyboxes[(int)Mathf.Floor((timeOfDay / secondsInDay) * Skyboxes.Length)];
    }

    void updateThoughtTimer()
    {
        if (thoughtTimer <= 0)
        {
            thoughtBox.SetActive(false);
        }
        else
        {
            thoughtTimer -= Time.deltaTime;
        }
    }

    void updateWorking()
    {
        if (working)
        {
            if (health <= MaxHP / 2 || isNight())
            {
                stopWorking();
            }
            else
            {
                float workDone = Time.deltaTime * workPerSecond;
                spendEnergy(workDone);
                money += workDone;
                timeOfDay += workDone;
            }
        }
    }

    bool isHungry()
    {
        return food <= 0;
    }

    bool isTired()
    {
        return energy <= 0;
    }

    void spendEnergy(float amount)
    {
        energy -= amount;
        food -= amount * foodLostPerEnergy;
        if (isTired())
        {
            health += energy;
            energy = 0;
            speedMultiplier *= 0.998f; //can go below 1?
        }
        if (isHungry())
        {
            health += food;
            food = 0;
            speedMultiplier *= 0.998f; //can go below 1?

            if (!hungry)
            {
                haveThought("I'm hungry. I should eat something.");
                hungry = true;
            }
        }
        else
        {
            hungry = false;
        }
    }

    void updateInfoDisplay()
    {
        moneyText.GetComponent<Text>().text = string.Format("Money: ${0}\nEnergy: {1}\nHealth: {2}{3}\nTo Do: {4}\nDay {5} Time {6}",
            money.ToString("F2"), energy.ToString("F1"), health.ToString("F1"),
            isHungry() ? " (hungry)" : "", needToWork ? "work" : "sleep", todaysDate.ToString(), timeOfDay.ToString("F1"));
    }

    void updateSpeech()
    {
        GameObject Talker = GetNearestNPCWithinRange(maxTalkingRange);
        if (Talker != null && availableShop == null /*don't talk near shops*/
            && (Input.GetKeyDown("e") || Talker.GetComponent<NPC>().talksFirst))
        {
            whosTalking = Talker;
            talkerBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetName();
            speechBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetSpeech();
            portrait.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Portraits/" + Talker.GetComponent<NPC>().GetPortrait());
            dialogueBox.SetActive(true);
            Talker.GetComponent<NPC>().Interact();
        }
        else if (Talker != whosTalking)
        {
            dialogueBox.SetActive(false);
        }
    }

    void checkInputs()
    {
        if (Input.GetKeyDown("c")) //debug cheat
        {
            speedMultiplier = maxSpeedMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleMusic();
        }

        if (Input.GetKeyDown("e") && availableShop != null)
        {
            availableShop.Show(todaysDate);
            if (MusicOn)
            {
                ToggleMusic();
            }
        }
    }

    public void ToggleMusic()
    {
        BeatSpawner.instance.Toggle();
        MusicOn = !MusicOn;
        MusicSys.PlayIfNeeded();
    }

    void limitBarMax()
    {
        if (energy > MaxEnergy)
        {
            energy = MaxEnergy;
        }
        if (health > MaxHP)
        {
            health = MaxHP;
        }
        if (food > MaxFood)
        {
            food = MaxFood;
        }
    }

    // Update is called once per frame
    void Update()
    {

        // TEST
        if (Input.GetKeyDown("i"))
        {
            Inventory.instance.Toggle();
        }

        updateTimeOfDay();
        updateThoughtTimer();
        updateWorking();
        updateInfoDisplay();
        updateSpeech();
        limitBarMax();
        HandleCameraPivot();

        spendEnergy(Time.deltaTime * energyLostPerSecond);

        checkInputs();

        FadeSong(MusicOn);

        if (boostDuration > 0)
        {
            speedMultiplier = Mathf.Max(speedMultiplier, 1 + speedBoost);
        }

        lastPosition = transform.position;
        Move();
    }

    void HandleCameraPivot()
    {
        if (Input.GetMouseButton(1)) // right click
        {
            float horizontal = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            //float vertical = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            transform.Rotate(0, horizontal, 0, Space.Self);
            Cursor.lockState = CursorLockMode.Locked; //TODO, replace cursor to sprite so it can be hidden smartly later
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void stopWorking()
    {
        haveThought("I should sleep");
        working = false;
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "Trigger")
        {
            // EnvObj
            EnvObj envObj = collider.gameObject.GetComponentInParent<EnvObj>();
            if (envObj != null)
            {
                envObj.Interact();
            }

        }
        if (collider.gameObject.name == "WorkTrigger")
        {
            if (needToWork)
            {
                working = true;
                needToWork = false;
                money += 50;
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
                if (timeOfDay > secondsInDay / 2)
                {
                    needToWork = true;
                    float sleep_amount = ((secondsInDay - timeOfDay) / secondsInDay) * 2 * MaxEnergy;
                    Debug.Log("Slept for " + sleep_amount.ToString("F1"));
                    energy = Mathf.Min(MaxEnergy, energy + sleep_amount);
                    timeOfDay = secondsInDay - 1;
                    haveThought("I should work");
                }
                else
                {
                    haveThought("It's too early to sleep");
                }
            }
            else
            {
                haveThought("I haven't gone to work yet");
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

        if (collider.gameObject.name == "WorkTrigger")
        {
            if (working)
            {
                stopWorking();
            }
        }
    }

    public void haveThought(string thought, float lifespan = 3f)
    {
        thoughtText.GetComponent<Text>().text = thought;
        thoughtBox.SetActive(true);
        thoughtTimer = lifespan;
    }

    void LateUpdate()
    {
        DebugInfo.text = "Speed multiplier: " + speedMultiplier.ToString();
        DebugInfo.text += "\nCurrent speed: " + (Vector3.Distance(transform.position, lastPosition)).ToString();
        DebugInfo.text += "\nTime of Day: " + ((timeOfDay / secondsInDay) * 24).ToString("F2");
    }

    //FIXME probably switch this to the normal way triggers happen, code is probably lighter that way when there's many NPCs
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
        if (boostDuration > 0)
        {
            moveDirection *= (1 + speedBoost);
        }
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
            if (boostDuration > 0)
            {
                moveDirection.y *= (1 + jumpMultiplier);
            }
            //gameObject.transform.Find("R_arm").gameObject //TODO
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
        speedMultiplier = Mathf.Max(speedMultiplier + .1f, Mathf.Min(speedMultiplier * ExcellentBeatMult, maxSpeedMultiplier));
    }

    public void GreatBeat()
    {
        speedMultiplier = Mathf.Max(speedMultiplier + .01f, Mathf.Min(speedMultiplier * GoodBeatMult, maxSpeedMultiplier));
    }

    public void GoodBeat()
    {
        speedMultiplier = Mathf.Max(speedMultiplier + .001f, Mathf.Min(speedMultiplier * GoodBeatMult, maxSpeedMultiplier));
    }

    public void OKBeat()
    {
        speedMultiplier = Mathf.Max(speedMultiplier + .0001f, Mathf.Min(speedMultiplier * OKBeatMult, maxSpeedMultiplier));
    }

    public void MissBeat()
    {
        speedMultiplier = Mathf.Min(speedMultiplier, Mathf.Max(speedMultiplier * MissBeatMult, 1));
    }

}
