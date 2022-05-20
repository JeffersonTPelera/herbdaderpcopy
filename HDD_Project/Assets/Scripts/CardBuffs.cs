using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardBuffs : MonoBehaviour
{
    private Player playerStats;
    private int attack, defense, health, lifeSteal, eCD, aBuff, dBuff;
    private float speed, sBuff, qCD;

    private GameController controller;
    
    //Each card has a specific stat to increase, and how much they increase varies 
    [Header("Normal Stat to Buff")]
    [SerializeField] private float specificAttack;
    [SerializeField] private float specificDefense;
    [SerializeField] private float specificHealth;
    [SerializeField] private float specificSpeed;
    [SerializeField] private float specificECD;
    [SerializeField] private int specificLifeSteal;
    [SerializeField] private float specificQCD;
    
    [Header("To Increase Buff From E")] 
    [SerializeField] private float specificBA;
    [SerializeField] private float specificBD;
    [SerializeField] private float specificBS;
    
    [Header("Flat Bool")]
    //isFlat returns a flat stat boost, !isFlat returns a percentage based stat boost
    [SerializeField] private bool isFlat;



    //Prevents object from being destroyed on load, so that player stats possibly don't get destroyed
     //Given that the starting scene is randomized, we have to find a way to avoid duplicates
     /* void Awake()
      {
          DontDestroyOnLoad(this.gameObject);
      }*/
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerStats = playerObj.GetComponent<Player>();

        GameObject gameObj = GameObject.Find("Game Controller");
        controller = gameObj.GetComponent<GameController>();
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }

    /*
     *  BUTTON FUNCTIONS 
     */
    
    public void BuffAttack()
    {
        Debug.Log("You have pressed attack!");
        int newAttack = 0;
        attack = playerStats.GetBaseAttack();
        Debug.Log(playerStats.GetBaseAttack());
        
        if (isFlat)
        {
            newAttack = attack +  (int) specificAttack;
        }
        else
        {
            newAttack = (int)Math.Ceiling(attack + (attack * specificAttack));
        }

        playerStats.SetBaseAttack(newAttack);
        Debug.Log(playerStats.GetBaseAttack());
        controller.OfferNextLevelAfter();
    }
    
    public void BuffDefense()
    {
        Debug.Log("You have pressed defense!");
        int newDefense = 0;
        defense = playerStats.GetBaseDefense();
        Debug.Log(playerStats.GetBaseDefense());

        if (isFlat)
        {
            newDefense = defense +  (int) specificDefense;
        }
        else
        {
            newDefense = (int)Math.Ceiling(defense + (defense * specificDefense));
        }

        playerStats.SetBaseDefense(newDefense);
        Debug.Log(playerStats.GetBaseDefense());
        controller.OfferNextLevelAfter();
    }

    public void BuffSpeed()
    {
        Debug.Log("You have pressed speed!");
        float newSpeed = 0;
        speed = playerStats.GetBaseSpeed();
        Debug.Log(playerStats.GetBaseSpeed());
        
        if (isFlat)
        {
            newSpeed = speed + specificSpeed;
        }

        else
        {
            newSpeed = (speed * specificSpeed) + speed;
        }
        
        playerStats.SetBaseSpeed(newSpeed);
        Debug.Log(playerStats.GetBaseSpeed());
        controller.OfferNextLevelAfter();
    }

    //Increases player's health
    public void BuffHealth()
    {
        Debug.Log("You have pressed health!");
        int newHealth = 0;
        health = playerStats.GetBaseHealth();
        Debug.Log(playerStats.GetBaseHealth());
        if (isFlat)
        {
            newHealth = health +  (int) specificHealth;
        }

        else
        {
            newHealth = (int) Math.Ceiling((health * specificHealth) + health);
        }
        
        playerStats.SetBaseHealth(newHealth);
        Debug.Log(playerStats.GetBaseHealth());
        controller.OfferNextLevelAfter();
    }

    public void BuffECoolDown()
    {
        Debug.Log("You have pressed E cooldown!");
        int newEcd = 0;
        
        eCD = playerStats.GetBuffCD();
        Debug.Log(playerStats.GetBuffCD());
        newEcd  = (int) Math.Ceiling(eCD - (eCD * specificECD));
        playerStats.SetBuffCD(newEcd);
        Debug.Log(playerStats.GetBuffCD());
        controller.OfferNextLevelAfter();
    }
    
    public void BuffLifeSteal()
    {
        Debug.Log("You have pressed life steal!");
        int newLifeSteal = 0;
        lifeSteal = playerStats.GetLifeSteal();
        Debug.Log(playerStats.GetLifeSteal());
        
        if (isFlat)
        {
            newLifeSteal = lifeSteal +  specificLifeSteal;
        }

        else
        {
            newLifeSteal = (lifeSteal *  specificLifeSteal) + lifeSteal;
        }
        
        playerStats.SetLifeSteal(newLifeSteal);
        Debug.Log(playerStats.GetLifeSteal());
        controller.OfferNextLevelAfter();
    }
    
    //Function for completely healing the player
    public void HealPlayer()
    {
        Debug.Log("You have pressed heal player!");
        //Debug.Log(playerStats.GetBaseHealth());
        playerStats.SetCurrHealth(playerStats.GetBaseHealth());
        //Debug.Log(playerStats.GetBaseHealth());
        controller.OfferNextLevelAfter();
    }

    public void BuffEStats()
    {
        Debug.Log("You have pressed buff E stats!");
        int newA, newD;
        float newS;
        
        aBuff = playerStats.GetBuffBA();
        dBuff = playerStats.GetBuffBD();
        sBuff = playerStats.GetBuffBS();

        Debug.Log(playerStats.GetBuffBA());
        Debug.Log(playerStats.GetBuffBD());
        Debug.Log(playerStats.GetBuffBS());
        
        newA = (int) Math.Ceiling((aBuff * specificBA) + aBuff);
        newD = (int) Math.Ceiling((dBuff * specificBD) + dBuff);
        newS = (sBuff * specificBS) + sBuff;
        
        playerStats.SetBuffBA(newA);
        playerStats.SetBuffBD(newD);
        playerStats.SetBuffBS(newS);
        Debug.Log(playerStats.GetBuffBA());
        Debug.Log(playerStats.GetBuffBD());
        Debug.Log(playerStats.GetBuffBS());
        controller.OfferNextLevelAfter();
    }

    public void BuffQCooldown()
    {
        Debug.Log("You have pressed to buff Q cooldown!");
        float newQcd;        
        qCD = playerStats.GetLSCD();
        Debug.Log(playerStats.GetLSCD());
        newQcd  = (int) Math.Ceiling(qCD - (qCD * specificQCD));
        playerStats.SetLSCD(newQcd);
        Debug.Log(playerStats.GetLSCD());
        controller.OfferNextLevelAfter();
    }
}
