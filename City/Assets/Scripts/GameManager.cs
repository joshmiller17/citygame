using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Global Settings")]
    public string buildID;
    [Range(0f, 1f)]
    public float PercentLightInDay;
    public float SongFadeTime;
    public bool MusicOn = false;
    public bool DebugMode = false;

    [Space(10)]
    [Header("Global Stats")]
    public int todaysDate = 0;
    public float timeOfDay;
    public int secondsInDay;

    [Space(10)]
    [Header("GameObj Links")]
    public Material BlendedSkybox;
    public Material[] Skyboxes;
    public Text DebugInfo;
    //public GameObject TPCamera;
    public GameObject MusicSysObj;
    //public Transform pivot;
    public GameObject dialogueBox;
    public GameObject talkerBox;
    public GameObject speechBox;
    public GameObject thoughtBox;
    public GameObject thoughtText;
    public GameObject moneyText;
    public GameObject interactionInfo;
    public MusicSystem MusicSys;
    public GameObject ShopCanvas;

    [Space(10)]
    [Header("Global Objects")]
    public GameObject cube;
    public GameObject capsule;
    public Material[] defaultMaterials;
    public Material[] happyMaterials;
    public GameObject trigger;
    public TraceryNameGenerator NameGen;

    private AudioSource aud;
    private GameObject whosTalking = null;
    private float thoughtTimer = 0;
    [HideInInspector] public Shop availableShop = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start() { 
        MusicSys = MusicSysObj.GetComponent<MusicSystem>();
        aud = MusicSys.GetComponent<AudioSource>();

        //TEST
        //Song s = MusicSys.LoadSong("Algorithm", 1, .03f);
        Song s = MusicSys.LoadSong("WaterWoodAndStone", 1.6f, .05f);
        MusicItem m = MusicItem.BaseItem();
        m.song = s;
        Inventory.instance.musicItems.Add(m);
        Song s2 = MusicSys.LoadSong("FeelGoodRock", 1, .07f);
        MusicItem m2 = MusicItem.BaseItem();
        m2.song = s2;
        Inventory.instance.musicItems.Add(m2);
        Song s3 = MusicSys.LoadSong("DubstepLight", 1, .1f);
        MusicItem m3 = MusicItem.BaseItem();
        m3.song = s3;
        Inventory.instance.musicItems.Add(m3);
        Song s4 = MusicSys.LoadSong("AcousticBlues", 1, .08f);
        MusicItem m4 = MusicItem.BaseItem();
        m4.song = s4;
        Inventory.instance.musicItems.Add(m4);
        Song s5 = MusicSys.LoadSong("BoogieWoogieBed", 2, .05f);
        MusicItem m5 = MusicItem.BaseItem();
        m5.song = s5;
        Inventory.instance.musicItems.Add(m5);
        Song s6 = MusicSys.LoadSong("GetAMoveOn", 2, .085f);
        MusicItem m6 = MusicItem.BaseItem();
        m6.song = s6;
        Inventory.instance.musicItems.Add(m6);
        MusicSys.SetSong(s);

        NameGen.Init();
    }

    public void Update()
    {
        updateTimeOfDay();
        updateThoughtTimer();
        updateInfoDisplay();
        updateSpeech();
        FadeSong(MusicOn);
    }

    void LateUpdate()
    {
        DebugInfo.text = "Build ID: " + buildID;
        DebugInfo.text = "Speed multiplier: " + PlayerController.instance.controller.m_MoveSpeedMultiplier.ToString("F2");
        DebugInfo.text += "\nTime of Day: " + ((timeOfDay / secondsInDay) * 24).ToString("F2");
    }

    void FadeSong(bool MusicOn)
    {
        if (MusicOn && aud.volume < .7) // TODO change to a variable set by player if they want to adjust music settings
        {
            aud.volume += Time.deltaTime / SongFadeTime;
        }
        else if (!MusicOn && aud.volume > 0)
        {
            aud.volume -= Time.deltaTime / SongFadeTime;
        }
    }

    public void haveThought(string thought, float lifespan = 3f)
    {
        thoughtText.GetComponent<Text>().text = thought;
        thoughtBox.SetActive(true);
        thoughtTimer = lifespan;
    }

    public bool isNight()
    {
        return timeOfDay >= secondsInDay * PercentLightInDay;
    }


    void updateInfoDisplay()
    {
        moneyText.GetComponent<Text>().text = string.Format("Money: ${0}\nEnergy: {1}\nHealth: {2}{3}\nTo Do: {4}\nDay {5} Time {6}",
            PlayerController.instance.money.ToString("F2"), PlayerController.instance.energy.ToString("F1"),
            PlayerController.instance.health.ToString("F1"), PlayerController.instance.isHungry() ? " (hungry)" : "", 
            PlayerController.instance.needToWork ? "work" : "sleep", todaysDate.ToString(), timeOfDay.ToString("F1"));
    }

    public void updateTimeOfDay()
    {
        timeOfDay += Time.deltaTime;
        if (timeOfDay > secondsInDay)
        {
            timeOfDay -= secondsInDay;
            todaysDate += 1;
        }

        float distanceFromNoon = Mathf.Abs(timeOfDay - (secondsInDay / 2));
        BlendedSkybox.SetFloat("_Blend", (1 - (distanceFromNoon / (secondsInDay / 2))));
        //RenderSettings.skybox = Skyboxes[(int)Mathf.Floor((timeOfDay / secondsInDay) * Skyboxes.Length)];
    }

    public void updateThoughtTimer()
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

    void updateSpeech()
    {
        GameObject Talker = GetNearestNPCWithinRange(PlayerController.instance.maxTalkingRange);
        if (Talker != null && availableShop == null /*don't talk near shops*/
            && (Input.GetKeyDown("e") || Talker.GetComponent<NPC>().talksFirst))
        {
            whosTalking = Talker;
            talkerBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetName();
            speechBox.GetComponent<Text>().text = Talker.GetComponent<NPC>().GetSpeech();
            dialogueBox.SetActive(true);
            Talker.GetComponent<NPC>().Interact();
        }
        else if (Talker != whosTalking)
        {
            dialogueBox.SetActive(false);
        }
    }

    //FIXME probably switch this to the normal way triggers happen, code is probably lighter that way when there's many NPCs
    GameObject GetNearestNPCWithinRange(float maxRangeAllowed)
    {
        GameObject[] npcs;
        npcs = GameObject.FindGameObjectsWithTag("NPC");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = PlayerController.instance.transform.position;
        foreach (GameObject npc in npcs)
        {
            Vector3 diff = npc.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && curDistance < maxRangeAllowed)
            {
                closest = npc;
                distance = curDistance;
            }
        }
        return closest;
    }


    public bool IsShopping()
    {
        return ShopCanvas.activeInHierarchy;
    }


    public void ToggleMusic()
    {
        BeatSpawner.instance.Toggle();
        MusicOn = !MusicOn;
        MusicSys.PlayIfNeeded();
    }

}
