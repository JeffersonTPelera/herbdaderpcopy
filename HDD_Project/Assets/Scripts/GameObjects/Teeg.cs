using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Teeg : MonoBehaviour
{
    private void WanderState() { }
    private void IdleState() { }
    private void AgroState() { }

    public GameObject dmgPopUp;

    //Base Stats
    private int baseHealth = 15;
    private int baseAttack = 10;
    private int baseDefense = 24;
    private float baseSpeed = 5;
    private int currHealth;

    [SerializeField] private float agroRange;
    [SerializeField] private GameObject bullet;
    public Animator teeg_anim;
    
    private float waitTime = .5f;
    private float wanderTime = .75f;
    private Vector3 wanderDest;
    private Quaternion rotation;

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
    //public Slider healthBar;
    public int damageTaken;

    private bool isTakingDamage = false;
    private EnemySpawner _enemySpawner;
    private float immuneTime = .25f;
    private SpriteRenderer teegRender;
    private CapsuleCollider2D teegCollider;
    private Player player;

    private ItemSpawner spawnItem;
    
    // Start is called before the first frame update
    void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        target = FindObjectOfType<Player>().transform;
        StartCoroutine("WanderTimer");
        teegRender = GetComponent<SpriteRenderer>();
        teegCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        SetStats();
        spawnItem = GameObject.FindGameObjectWithTag("Item Spawner").GetComponent<ItemSpawner>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (player.transform.position.x < transform.position.x)
            {
                //player is to the left
                isLeft = true;
            }
            else
            {
                //player is to the right
                isLeft = false;
            }
        
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
        else if (Vector3.Distance(target.position, transform.position) < 6 && !isIdle && !isAttacking && canAttack)//to close
        {
            StartCoroutine(AttackAnim());
            isWandering = false;
            isAgro = false;
        }
        else if (Vector3.Distance(target.position, transform.position) < 6 && !isIdle && !isAttacking && !canAttack)
        {
            isIdle = true;
            isMoving = false;
            StartCoroutine(IdleTimer());
        }
        else if (Vector3.Distance(target.position, transform.position) <= agroRange && !isIdle)//in agro range
        {
            isAgro = true;
            isWandering = false;
        }
    }

    void Animate()
    {
        teeg_anim.SetBool("isLeft", isLeft);
        teeg_anim.SetBool("isAttacking", isAttacking);
        teeg_anim.SetBool("isMoving", isMoving);
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
                    teegCollider.enabled = false;
                    teegRender.enabled = false;
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
        for (int i = -1; i < 10; i++)
        {
            if (i >= 0 && i < 3)
            {
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 0.3f, 0);
                Vector3 enemyToPlayer = new Vector3((player.transform.position.x - transform.position.x), (player.transform.position.y - transform.position.y + 0.3f), 0);
                enemyToPlayer = enemyToPlayer.normalized;
                float attackAngle = (Mathf.Atan2(enemyToPlayer.y, enemyToPlayer.x) * Mathf.Rad2Deg);
                if (attackAngle <= -170 && attackAngle >= -180 || attackAngle >= 170 && attackAngle <= 180 || attackAngle <= 0 && attackAngle >= -10 || attackAngle >= 0 && attackAngle <= 10)
                {
                    rotation = Quaternion.identity;
                }
                else
                {
                    rotation = Quaternion.Euler(0,0,attackAngle);
                }
                Instantiate(bullet, spawnLoc, rotation,this.gameObject.transform);
                Debug.Log(attackAngle);
            }
            else if (i == 3)
            {
                isAttacking = false;
                canAttack = false;
            }
            else if (i == 9)
            {
                canAttack = true;
            }
            yield return new WaitForSeconds(.3f);
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

    private IEnumerator IdleTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isIdle = false;
                isMoving = true;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void SetStats()
    {
        int lvl = player.GetNumLvlsCompleted();
        if (lvl < 5)
        {
            baseHealth = 20 + (int)(lvl * 8);
            baseAttack = 10 + (int)(lvl * 1.5);
            baseDefense = 24 + (int)(lvl*3);
            baseSpeed = 4 + (lvl * .01f);
            currHealth = baseHealth;
        }
        else
        {
            baseHealth = 25 + (int)(lvl * 12);
            baseAttack = 10 + (int)(lvl * 2);
            baseDefense = 28 + (int)(lvl * 3);
            baseSpeed = 4 + (lvl * .02f);
            currHealth = baseHealth;
        }
    }
}
