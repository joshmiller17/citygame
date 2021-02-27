using System.Collections;
using System.Collections.Generic;
using TurnTheGameOn.ArrowWaypointer;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Header("Player Settings")]
    public float energyLostPerSecond = 0.1f;
    public float foodLostPerEnergy = 1.5f;
    public float workPerSecond = 5;
    public float workTimeFastForwardSpeed = 10;
    public float maxTalkingRange = 70;
    public float minimumSpeed = 0.5f;


    [Space(10)]
    [Header("Player Stats")]
    public int MaxHP = 100;
    public int MaxEnergy = 100;
    public int MaxFood = 100;
    public float money = 100;
    public float energy = 100;
    public float health = 100;
    public float food = 100;
    public float boostDuration = 0;
    public float speedBoost = 0;
    public float jumpBoost = 0;

    [Space(10)]
    public CharacterControllerThirdPerson controller;
    public SmoothFollow PlayerCamera;

    [HideInInspector] public bool hungry = false;
    [HideInInspector] public bool needToWork = true;
    [HideInInspector] public bool working = false;

    void Start()
    {
        instance = this;
        GameManager.instance.haveThought("I should go to work");
    }

    void updateWorking()
    {
        if (working)
        {
            if (energy <= 0 || food <= 0 || health <= MaxHP / 2 || GameManager.instance.isNight())
            {
                stopWorking();
            }
            else
            {
                float workDone = Time.deltaTime * workPerSecond;
                spendEnergy(workDone);
                money += workDone;
                GameManager.instance.timeOfDay += Time.deltaTime * workTimeFastForwardSpeed;
            }
        }
    }

    public bool isHungry()
    {
        return food <= 50;
    }

    public bool isTired()
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
            controller.m_MoveSpeedMultiplier *= 0.999f;
        }
        if (isHungry())
        {
            if (!hungry)
            {
                GameManager.instance.haveThought("I'm hungry. I should eat something.");
                hungry = true;
            }

            if (food < 0)
            {
                controller.m_MoveSpeedMultiplier *= 0.999f;
                health += food;
                food = 0;
            }
        }
        else
        {
            hungry = false;
        }

        // cap speed loss
        controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier, minimumSpeed);
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
        //slow down if not moving fast or music off
        if (Input.GetAxis("Vertical") < 0.5 || !GameManager.instance.MusicOn)
        {
            controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier * .999f, 1);
        }

        boostDuration = Mathf.Max(boostDuration - Time.deltaTime, 0);
        updateWorking();
        limitBarMax();
        spendEnergy(Time.deltaTime * energyLostPerSecond);

        // adjust camera to speed
        PlayerCamera.distance = 4 + 1.5f * controller.m_MoveSpeedMultiplier;
        PlayerCamera.height = 4 + 1.5f * controller.m_MoveSpeedMultiplier;
        PlayerCamera.rotationDamping = 4 + 1.5f * controller.m_MoveSpeedMultiplier;
        PlayerCamera.heightDamping = 4 + 1.5f * controller.m_MoveSpeedMultiplier;

        if (boostDuration > 0)
        {
            controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier, 1 + speedBoost);
            controller.m_JumpBoost = jumpBoost;
        }
        else
        {
            controller.m_JumpBoost = 0;
        }
    }



    void stopWorking()
    {
        GameManager.instance.haveThought("I should sleep");
        working = false;
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name.Contains("Trigger"))
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
                GameManager.instance.haveThought("I already worked today, I should sleep");
            }
        }

        if (collider.gameObject.name == "HomeTrigger")
        {
            if (!needToWork)
            {
                if (GameManager.instance.timeOfDay > GameManager.instance.secondsInDay / 2)
                {
                    needToWork = true;
                    float sleep_amount = ((GameManager.instance.secondsInDay - GameManager.instance.timeOfDay) / GameManager.instance.secondsInDay) * 2 * MaxEnergy;
                    Debug.Log("Slept for " + sleep_amount.ToString("F1"));
                    energy = Mathf.Min(MaxEnergy, energy + sleep_amount);
                    GameManager.instance.timeOfDay = GameManager.instance.secondsInDay - 1;
                    GameManager.instance.haveThought("I should work");
                }
                else
                {
                    GameManager.instance.haveThought("It's too early to sleep");
                }
            }
            else
            {
                GameManager.instance.haveThought("I haven't gone to work yet");
            }
        }

        if (collider.gameObject.name == "ShopTrigger")
        {
            GameManager.instance.availableShop = collider.gameObject.GetComponentInParent<Shop>();
            GameManager.instance.interactionInfo.GetComponent<Text>().text = "Press E to Shop";
            GameManager.instance.interactionInfo.SetActive(true);
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "ShopTrigger")
        {
            GameManager.instance.availableShop = null;
            GameManager.instance.interactionInfo.SetActive(false);
        }

        if (collider.gameObject.name == "WorkTrigger")
        {
            if (working)
            {
                stopWorking();
            }
        }
    }


    public void ExcellentBeat()
    {
        controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier + .1f, Mathf.Min(controller.m_MoveSpeedMultiplier * ExcellentBeatMult, controller.m_MaxMoveSpeedMultiplier + speedBoost));
    }

    public void GreatBeat()
    {
        controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier + .01f, Mathf.Min(controller.m_MoveSpeedMultiplier * GoodBeatMult, controller.m_MaxMoveSpeedMultiplier + speedBoost));
    }

    public void GoodBeat()
    {
        controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier + .001f, Mathf.Min(controller.m_MoveSpeedMultiplier * GoodBeatMult, controller.m_MaxMoveSpeedMultiplier + speedBoost));
    }

    public void OKBeat()
    {
        controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier + .0001f, Mathf.Min(controller.m_MoveSpeedMultiplier * OKBeatMult, controller.m_MaxMoveSpeedMultiplier + speedBoost));
    }

    public void MissBeat()
    {
        controller.m_MoveSpeedMultiplier = Mathf.Min(controller.m_MoveSpeedMultiplier, Mathf.Max(controller.m_MoveSpeedMultiplier * MissBeatMult, 1));
    }

}
