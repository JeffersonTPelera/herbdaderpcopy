using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using System;

public class Geejus : MonoBehaviour
{
    private void WanderState() { }
    private void IdleState() { }
    private void AgroState() { }

    public GameObject dmgPopUp;

    //Base Stats
    private int baseHealth = 50;
    private int baseAttack = 15;
    private int baseDefense = 30;
    private float baseSpeed = 5;
    private int currHealth;

    [SerializeField] private float agroRange;
    [SerializeField] private CapsuleCollider2D damageBox;
    [SerializeField] private GameObject puke;
    private float waitTime = .5f;
    private float wanderTime = .75f;
    private Vector3 wanderDest;

    private bool isAttacking = false,
        isAgro = false,
        isWandering = false,
        isIdle = false,
        isLeft = true,
        isPuking = false,
        isMoving = true,
        isDead;
    
    private float stopAgroRange = .75f;

    private Transform target;
    public int damageTaken;
    
    private bool isTakingDamage = false;
    private EnemySpawner _enemySpawner;
    private float immuneTime = .25f;

    private SpriteRenderer geejusRender;
    private CircleCollider2D geejusCollider;
    private Player player;

    private float lastXPos = 0;
    private float currX;
    
    public Animator geejus_anim;

    private ItemSpawner spawnItem;
    
    // Start is called before the first frame update
    void Start()
    {
        geejusRender = GetComponent<SpriteRenderer>();
        geejusCollider = GetComponent<CircleCollider2D>();
        geejus_anim = GetComponent<Animator>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        target = FindObjectOfType<Player>().transform;
        StartCoroutine("WanderTimer");
        currX = transform.position.x;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        SetStats();
        //currY = transform.position.y;
        //StartCoroutine(PukingTimer());

        spawnItem = GameObject.FindGameObjectWithTag("Item Spawner").GetComponent<ItemSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (player.transform.position.x > transform.position.x)
            {
                isLeft = false;
            }
            else
            {
                isLeft = true;
            }
            
        
            PlayerIsInRange();
            if (isAgro)
            {
                if (!isPuking && !isIdle)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, baseSpeed * Time.deltaTime);
                }
                else
                {
                    Debug.Log("puking");
                }
            }
            else if (isWandering)
            {
                transform.position = Vector3.MoveTowards(transform.position, wanderDest, baseSpeed * Time.deltaTime);
            }
            Animate();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitBox"))
        {
            if (!isTakingDamage && !isDead)
            {
                damageTaken = (int)Math.Floor(player.GetBaseAttack() - (.25f * baseDefense))+1;
                if (damageTaken < 1)
                {
                    damageTaken = 1;
                }
                currHealth -= damageTaken;
                if (player.GetLSActive())
                {
                    player.SetCurrHealth((int)(player.GetCurrHealth() + (player.GetBaseAttack() * player.GetLSPercent())));//heals the player on damage of the enemy
                }
                GameObject dmgpu = Instantiate(dmgPopUp, transform.position, Quaternion.identity) as GameObject;
                string dmg = damageTaken + "";
                dmgpu.transform.GetComponent<TextMeshPro>().text = dmg;

                if (player.GetBaseHealth() > (player.GetLifeSteal() + player.GetCurrHealth()))
                {
                    player.SetCurrHealth(player.GetCurrHealth() + player.GetLifeSteal());
                }
                else
                {
                    player.SetCurrHealth(player.GetBaseHealth());
                }

                if (currHealth <= 0)
                {
                    Debug.Log("Enemy was just killed");
                    _enemySpawner.SetNumENemies(_enemySpawner.GetNumEnemies() - 1);
                    StartCoroutine(OnDeath());
                    geejusCollider.enabled = false;
                    geejusRender.enabled = false;
                    isDead = true;

                    //50 percent chance to spawn item
                    //Random.value returns a float between 0 to 1,
                    //so Random.value > 0.2 would return 80% because 1 - 0.2 is 80%
                    if (Random.value > 0.5)
                    {
                        spawnItem.SpawnItem(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
                    }
                    
                    //Destroy(this.gameObject);
                }
            }
            StartCoroutine("damageTimer");
        }
    }

    private IEnumerator damageTimer()
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(immuneTime);
        isTakingDamage = false;
    }
    
    private void PlayerIsInRange()
    {
        if (Vector3.Distance(target.position, transform.position) > agroRange)
        {
            isAgro = false;
            if (!isWandering)
            {
                StartCoroutine("WanderTimer");//sets isWandering to true and wanders till in agro state
            }
        }
        else if (Vector3.Distance(target.position, transform.position) < 5 && !isIdle)
        {
            isIdle = true;
            StartCoroutine(PukingAnim());
            isWandering = false;
            isAgro = false;
            StartCoroutine(IdleTimer());
        }
        else if (Vector3.Distance(target.position, transform.position) <= agroRange && !isIdle)
        {
            isAgro = true;
            isWandering = false;
        }
    }

    public void onPlayerDamage()
    {
        damageBox.enabled = false;
        StartCoroutine("damageBoxTimer");
    }
    
    public int GetAttack()
    {
        return baseAttack;
    }

    // Setting up values for animation
    void Animate()
    {
        geejus_anim.SetBool("IsLeft", isLeft);
        geejus_anim.SetBool("isPuking", isPuking);
        geejus_anim.SetBool("isMoving", isMoving);
    }

    private IEnumerator damageBoxTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                damageBox.enabled = true;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator WanderTimer()
    {
        isWandering = true;
        while (!isAgro)
        {
            wanderTime = Random.Range(1.5f, 4f);
            wanderDest = new Vector3(transform.position.x + Random.Range(-1,2), transform.position.y + Random.Range(-1,2), 0);
            yield return new WaitForSeconds(wanderTime);
        }
    }

    private IEnumerator PukingTimer()
    {
        int rand;
        while (true)
        {
            Debug.Log("puking timer cycle");
            if (!isPuking)
            {
                rand = Random.Range(1, 101);//rand number from 1 to 100
                if (isAgro && rand < 33)//% chance
                {
                    StartCoroutine(PukingAnim());
                }
            }
            yield return new WaitForSeconds(3);
        }
    }

    private IEnumerator PukingAnim()
    {
        isPuking = true;
        Vector3 spawnLoc = new Vector3();
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isPuking = false;
                spawnLoc = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            }
            yield return new WaitForSeconds(.5f);
        }
        StartCoroutine(PukeDespawmTimer(Instantiate(puke, spawnLoc, quaternion.identity)));
    }

    private IEnumerator IdleTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isIdle = false;
            }
            yield return new WaitForSeconds(4);
        }
    }

    private IEnumerator PukeDespawmTimer(GameObject pukeInst)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                Destroy(pukeInst);
            }
            yield return new WaitForSeconds(6);
        }
    }

    private IEnumerator OnDeath()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(15);
        }
    }
    
    private void SetStats()
    {
        int lvl = player.GetNumLvlsCompleted();
        if (lvl < 6)
        {
            baseHealth = 75 + (int)(lvl * 5);
            baseAttack = 15 + (int)(lvl * 3);
            baseDefense = 32 + (int)(lvl * 2);
            baseSpeed = 4.5f + (lvl * .01f);
            currHealth = baseHealth;
        }
        else
        {
            baseHealth = 80 + (int)(lvl * 10);
            baseAttack = 20 + (int)(lvl * 3.5f);
            baseDefense = 32 + (int)(lvl * 4);
            baseSpeed = 4.5f + (lvl * .01f);
            currHealth = baseHealth;
        }
    }
}
