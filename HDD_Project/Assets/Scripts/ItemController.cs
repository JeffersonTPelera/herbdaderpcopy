using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    private bool collected;
    private SpriteRenderer itemSprite;

    //Normal stats
    private float norSpeed;
    private int norDamage;
    private int norDefense;

    private bool speedIsBuffed = false;
    private bool attackIsBuffed = false;
    private bool defenseIsBuffed = false;
    
    //Information from the player object
    private Player playerStats;
    private SpriteRenderer heroSprite;
    
    private Geej enemyHealth;
    
    //Item Types
    [SerializeField] private bool isHealth, isSpeed, isDmg,isDefense;

    // Start is called before the first frame update
    void Start()
    {
        itemSprite = GetComponent<SpriteRenderer>();
        
        //Finds game object with Player so that the code can get information from the Player object partaining to the Scripts
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerStats = playerObj.GetComponent<Player>();
        heroSprite = playerObj.GetComponent<SpriteRenderer>();

        //Most likely change Geej later on or add more tags
       /* GameObject enemyObj = GameObject.FindGameObjectWithTag("Geej"); 
        enemyHealth = enemyObj.GetComponent<Geej>();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator SpeedBuff()
    {
        TurnItemTransparent();
        collected = true;
        
        //Debug.Log("Time before yield: "+ Time.time);
        
        //norSpeed = playerStats.GetBaseSpeed();

        //Increases player speed
        //playerStats.SetBaseSpeed(norSpeed + 0.6f);
        playerStats.IncSpeed();
        speedIsBuffed = true;
        yield return new WaitForSeconds(10f);
        playerStats.DecSpeed();
        speedIsBuffed = false;
        //Debug.Log("Time after yield: "+ Time.time);
        
        //Player's speed goes back to normal after the amount of seconds in WaitForSeconds
        //playerStats.SetBaseSpeed(norSpeed);
        
        DestroyItem(); //Could't destroy the game object before then or everything after WaitForSeconds won't work
    }

    IEnumerator DamageBuff()
    {
        TurnItemTransparent();
        collected = true;
        
        //Debug.Log("Player's damage is buffed!");
        
        //norDamage = enemyHealth.damageTaken;
        //norDamage = playerStats.GetBaseAttack();

        //Increases player's damage against enemy
        //enemyHealth.damageTaken *= 3;
        //int tempDmg = (int)Math.Floor(norDamage * 1.2);
        //playerStats.SetBaseAttack(tempDmg);
        playerStats.IncAttack();
        attackIsBuffed = true;
        yield return new WaitForSeconds(10f);
        playerStats.DecAttack();
        attackIsBuffed = false;
        //Player's damage goes back to normal after the amount of seconds in WaitForSeconds
       //enemyHealth.damageTaken = norDamage;
       //playerStats.SetBaseAttack(norDamage);
       
        //Debug.Log("Player's buff damage is gone!");
        DestroyItem();
    }

    IEnumerator DefenseBuff()
    {
        TurnItemTransparent();
        collected = true;
        
        //heroSprite.color = new Color(heroSprite.color.r, heroSprite.color.g, heroSprite.color.b, .5f);

        //Debug.Log("Player's defense is buffed!");
        //norDefense = playerStats.GetBaseDefense();

        //Increases player defense
        //int tempDefence = (int)Math.Floor(norDefense * 1.2);
        //playerStats.SetBaseDefense(tempDefence);
        playerStats.IncDefense();
        defenseIsBuffed = true;
        yield return new WaitForSeconds(10f);
        playerStats.DecDefense();
        defenseIsBuffed = false;
        //Debug.Log("Player's defense buff is gone!");
        
        //Set back player's defense back to normal
        //playerStats.SetBaseDefense(norDefense);
        
        //heroSprite.color = new Color(heroSprite.color.r, heroSprite.color.g, heroSprite.color.b, 1f);
        
        DestroyItem();
    }
    
    //Function to turn item transparent in order to indicate that the player can't pick up the item anymore
    private void TurnItemTransparent()
    {
        itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, .5f);
    }
    
    //Function to use after item is collected
    private void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void EndOfLvlCleanup()
    {
        if (defenseIsBuffed)
        {
            playerStats.DecDefense();
        }
        if (attackIsBuffed)
        {
            playerStats.DecAttack();
        }
        if (speedIsBuffed)
        {
            playerStats.DecSpeed();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Makes sures that enemy can't trigger item
        if (other.CompareTag("Player"))
        {
            if (!collected)
            {
                if (isHealth)
                {
                    int heal = (int)(playerStats.GetCurrHealth() + (playerStats.GetBaseHealth() * .1));
                    if (playerStats.GetBaseHealth() > heal)
                    {
                        playerStats.SetCurrHealth(heal);
                    }
                    else
                    {
                        playerStats.SetCurrHealth(playerStats.GetBaseHealth());
                    }
                    collected = true;
                    DestroyItem();
                }

                if (isSpeed)
                {
                    StartCoroutine("SpeedBuff");
                }

                if (isDefense)
                {
                    StartCoroutine("DefenseBuff");
                }

                if (isDmg)
                {
                    StartCoroutine("DamageBuff");
                }
            }
        }

    }

   
}
