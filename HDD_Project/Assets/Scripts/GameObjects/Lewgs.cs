using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class Lewgs : MonoBehaviour
{
    private void WanderState() { }
    private void IdleState() { }
    private void AgroState() { }

    public GameObject dmgPopUp;

    //Base Stats
    private int baseHealth = 25;
    private int baseAttack = 15;
    private int baseDefense = 24;
    private float baseSpeed = 4.5f;
    private int currHealth;

    [SerializeField] private float agroRange;
    [SerializeField] private GameObject attackBox;
    public Animator lewgs_anim;
    
    private float waitTime = .5f;
    private float wanderTime = .75f;
    private Vector3 wanderDest;

    private float lastXPos = 0;
    private float currX;
    
    private bool isAttacking = false,
        isAgro = false,
        isWandering = false,
        isIdle = false,
        isLeft = true,
        isMoving = true,
        isDead,
        canAttack = true;

    private float stopAgroRange = .75f;

    private Transform target;
    public int damageTaken;

    private bool isTakingDamage = false;
    private EnemySpawner _enemySpawner;
    private float immuneTime = .25f;
    private SpriteRenderer lewgsRender;
    private CapsuleCollider2D lewgsCollider;
    private Player player;

    private ItemSpawner spawnItem;
    
    // Start is called before the first frame update
    void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        target = FindObjectOfType<Player>().transform;
        StartCoroutine("WanderTimer");
        lewgsRender = GetComponent<SpriteRenderer>();
        lewgsCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        SetStats();
        spawnItem = GameObject.FindGameObjectWithTag("Item Spawner").GetComponent<ItemSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            currX = transform.position.x;
            if (currX == lastXPos)
            {
                isMoving = false;
            }
            else
            {
                isMoving = true;
            }
            //currY = transform.position.y;
            if (currX > lastXPos)//right
            {
                isLeft = false;
            }
            else if (currX < lastXPos)//left
            {
                isLeft = true;
            }
            lastXPos = currX;
            //lastYPos = currY;

        
            PlayerIsInRange();
            if (isAgro)
            {
                if (!isAttacking && !isIdle)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, baseSpeed * Time.deltaTime);
                }
                else
                {
                    //Debug.Log("Attacking");
                }
            }
            else if (isWandering)
            {
                transform.position = Vector3.MoveTowards(transform.position, wanderDest, baseSpeed * Time.deltaTime);
            }
            Animate();
        }
    }
    
    private void PlayerIsInRange()
    {
        if (Vector3.Distance(target.position, transform.position) > agroRange)//out of range
        {
            isAgro = false;
            if (!isWandering)
            {
                StartCoroutine("WanderTimer");//sets isWandering to true and wanders till in agro state
            }
        }
        else if (Vector3.Distance(target.position, transform.position) < 1 && !isIdle && !isAttacking && canAttack)//to close
        {
            StartCoroutine(AttackAnim());
            isWandering = false;
            isAgro = false;
        }
        else if (Vector3.Distance(target.position, transform.position) <= agroRange && !isIdle)//in agro range
        {
            isAgro = true;
            isWandering = false;
        }
    }

    void Animate()
    {
        lewgs_anim.SetBool("isLeft", isLeft);
        lewgs_anim.SetBool("isAttacking", isAttacking);
        lewgs_anim.SetBool("isMoving", isMoving);
    }
    

    public int GetAttack()
    {
        return baseAttack;
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
                    lewgsCollider.enabled = false;
                    lewgsRender.enabled = false;
                    isDead = true;
                    
                    if (Random.value > 0.5)
                    {
                        spawnItem.SpawnItem(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
                    }
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

    private IEnumerator AttackAnim()
    {
        isAttacking = true;
        for (int i = 0; i < 7; i++)
        {
            if (i == 1)
            {
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y, 0);
                Instantiate(attackBox, spawnLoc, quaternion.identity);
            }
            else if (i == 6)
            {
                isAttacking = false;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    
    private IEnumerator OnDeath()//waits 10 seconds to destroy gameObjext to make sure attacks get destroyed as well
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(10);
        }
    }
    
    private void SetStats()
    {
        int lvl = player.GetNumLvlsCompleted();
        if (lvl < 5)
        {
            baseHealth = 50 + (int)(lvl * 5);
            baseAttack = 15 + (int)(lvl * 3);
            baseDefense = 24 + (int)(lvl * 1.5);
            baseSpeed = 2 + (lvl * .01f);
            currHealth = baseHealth;
        }
        else
        {
            baseHealth = 50 + (int)(lvl *10);
            baseAttack = 15 + (int)(lvl * 4);
            baseDefense = 24 + (int)(lvl * 3);
            baseSpeed = 2 + (lvl * .01f);
            currHealth = baseHealth;
        }
    }
}
