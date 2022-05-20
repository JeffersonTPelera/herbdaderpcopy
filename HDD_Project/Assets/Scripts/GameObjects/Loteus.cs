using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Loteus : MonoBehaviour
{
    private void WanderState() { }
    private void IdleState() { }
    private void AgroState() { }

    public GameObject dmgPopUp;

    //Base Stats
    private int baseHealth = 15;
    private int baseAttack = 20;
    private int baseDefense = 10;
    private float baseSpeed = 5;
    private int currHealth;

    [SerializeField] private float agroRange;
    [SerializeField] private GameObject disk;
    //[SerializeField] private CapsuleCollider2D damageBox;
    public Animator loteus_anim;
    
    private float waitTime = .5f;
    private float wanderTime = .75f;
    private Vector3 wanderDest;

    private float lastXPos = 0;
    private float currX;
    
    private bool isAttacking = false,
        isAgro = false,
        isWandering = false,
        isIdle = false,
        isMoving = true,
        isDead,
        canAttack = true;

    public bool isLeft = true;

    private float stopAgroRange = .75f;

    private Transform target;
    //public Slider healthBar;
    public int damageTaken;

    private bool isTakingDamage = false;
    private EnemySpawner _enemySpawner;
    private float immuneTime = .25f;
    private SpriteRenderer loteusRender;
    private CircleCollider2D loteusCollider;
    private Player player;
    
    private ItemSpawner spawnItem;

    // Start is called before the first frame update
    void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        target = FindObjectOfType<Player>().transform;
        StartCoroutine("WanderTimer");
        loteusRender = GetComponent<SpriteRenderer>();
        loteusCollider = GetComponent<CircleCollider2D>();
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
            if (currX > lastXPos && isMoving)//right
            {
                isLeft = false;
            }
            else if (currX < lastXPos && isMoving)//left
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
    
    private void PlayerIsInRange()
    {
        if (Vector3.Distance(target.position, transform.position) > agroRange)//out of range
        {
            isAgro = false;
            if (!isWandering && !isIdle)
            {
                StartCoroutine("WanderTimer");//sets isWandering to true and wanders till in agro state
            }
        }
        else if (Vector3.Distance(target.position, transform.position) < 3 && !isIdle && canAttack)//in attack range
        {
            isIdle = true;
            StartCoroutine(AttackAnim());
            StartCoroutine(AttackCooldown());
            isWandering = false;
            isAgro = false;
        }
        else if (Vector3.Distance(target.position, transform.position) <= agroRange && !isIdle)//in agro range
        {
            isAgro = true;
            isWandering = false;
        }
        else//in attack range but cant attack again yet
        {
            isIdle = true;
        }
    }

    void Animate()
    {
        loteus_anim.SetBool("isLeft", isLeft);
        loteus_anim.SetBool("isAttacking", isAttacking);
        loteus_anim.SetBool("isMoving", isMoving);
    }

    public int GetAttack()
    {
        return baseAttack;
    }

    public void SetIsIdle(bool newIdle)
    {
        isIdle = newIdle;
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
                    loteusCollider.enabled = false;
                    loteusRender.enabled = false;
                    isDead = true;
                    //Destroy(this.gameObject);
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
    
    private IEnumerator damageBoxTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                //damageBox.enabled = true;
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
    
    private IEnumerator AttackAnim()
    {
        isAttacking = true;
        yield return new WaitForSeconds(.5f);
        Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + .1f, 0);
        Instantiate(disk, spawnLoc, quaternion.identity, this.gameObject.transform);
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isAttacking = false;
                //TODO: spawn attack
            }
            yield return new WaitForSeconds(.5f);
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
    
    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                canAttack = true;
            }
            yield return new WaitForSeconds(3);
        }
    }
    
    private void SetStats()
    {
        int lvl = player.GetNumLvlsCompleted();
        if (lvl < 5)
        {
            baseHealth = 20 + (int)(lvl * 5);
            baseAttack = 20 + (int)(lvl * 2.5);
            baseDefense = 10 + (int)(lvl*1);
            baseSpeed = 2.5f;
            currHealth = baseHealth;
        }
        else
        {
            baseHealth = 25 + (int)(lvl * 10);
            baseAttack = 25 + (int)(lvl * 4);
            baseDefense = 10 + (int)(lvl * 2);
            baseSpeed = 2.5f;
            currHealth = baseHealth;
        }
    }

    public void IdleTimerStart()
    {
        StartCoroutine(SetIdleFalse());
    }
    private IEnumerator SetIdleFalse()
    {
        yield return new WaitForSeconds(1);
        isIdle = false;
    }
}
