using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static int baseHealth = 100;
    private static int baseAttack = 18;
    private static int maxDefense = 96;
    private static int baseDefense = 32;
    private static float baseSpeed = 5;
    private static int currHealth = baseHealth;
    private static int lifeSteal = 1;

    private String healthMsg = "Max Health: ";
    private String attackMsg = "Attack: ";
    private String defenseMsg = "Defense: ";
    private String speedMsg = "Speed: ";
    private String lifestealMsg = "Flat Lifesteal: ";
    private String dashCDMsg = "Dash CD: ";
    private String eCDMsg = "E CD: ";
    private String qCDMsg = "Q CD: ";
    
    [SerializeField] private TextMeshProUGUI healthStatDisplay;
    [SerializeField] private TextMeshProUGUI attackStatDisplay;
    [SerializeField] private TextMeshProUGUI defenseStatDisplay;
    [SerializeField] private TextMeshProUGUI speedStatDisplay;
    [SerializeField] private TextMeshProUGUI fLSStatDisplay;
    [SerializeField] private TextMeshProUGUI dashCDDisplay;
    [SerializeField] private TextMeshProUGUI eCDDisplay;
    [SerializeField] private TextMeshProUGUI qCDDisplay;

    private static int numLvlsCompleted = 0;

    [SerializeField] private GameObject hitbox;
    [SerializeField] private Vector2 spawnLoc;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;
    private float waitTime = .175f;
    private float attackAnimTime = .1f;

    private bool isAttacking = false;
    private bool isPreAttack = false;
    private bool canDash = true;
    private bool canBuff = true;
    private bool isDashing;
    private bool isBuffing;
    private bool isBuffed = false;
    private Vector2 initDashPos;
    private Vector2 dashTarget;
    private int dashCooldown = 3;
    private int dashRemainingCD = 0;
    private int buffCooldown = 25;
    private int buffRemainingCD = 0;
    private int buffDuration = 10;
    private int buffAttack = 3;
    private int buffDefense = 4;
    private float buffSpeed = 1;
    private int dashSpeed = 15;
    
    private bool canLS = true;
    private bool lSActive = false;
    private float lSPercent = .25f;
    private float lSCooldown = 15;
    private float lSRemainingCD = 0;

    private Vector3 attackVector;
    private Vector3 finalVector;
    private float attackAngle;
    public Slider healthBar; //Public so Item Controller can check if it's at the max value

    public Rigidbody2D rb;
    public Animator anim;
    private Camera cam;

    private UIController uic;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.maxValue = baseHealth;
        healthBar.value = currHealth;
        transform.position = spawnLoc;
        Debug.Log("base: " + baseHealth + " curr: " + currHealth);
        cam = GetComponentInChildren<Camera>();
        uic = gameObject.GetComponent<UIController>();
        hitbox.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        hitbox.GetComponent<BoxCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (baseDefense > maxDefense) baseDefense = maxDefense;
        healthBar.value = currHealth;
        ProcessInputs();
        Animate();
        
        UpdateStats();
        UpdateCDS();
    }

    public void UpdateStats()
    {
        healthStatDisplay.text = healthMsg + baseHealth;
        attackStatDisplay.text = attackMsg + baseAttack;
        defenseStatDisplay.text = defenseMsg + baseDefense;
        speedStatDisplay.text = speedMsg + baseSpeed;
        fLSStatDisplay.text = lifestealMsg + lifeSteal;
    }

    public void UpdateCDS()
    {
        dashCDDisplay.text = dashCDMsg + dashRemainingCD;
        eCDDisplay.text = eCDMsg + buffRemainingCD;
        qCDDisplay.text = qCDMsg + lSRemainingCD;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int damage;
        if (!isDashing && (other.CompareTag("Enemy") || other.CompareTag("Vomit") || other.CompareTag("Disk") ||
            other.CompareTag("LewgsAttack") || other.CompareTag("TeegBullet") || other.CompareTag("GeejusVomit") || 
            other.CompareTag("bigGeejVomit") || other.CompareTag("BigGeejBullet")))
        {
            //healthBar.value -= 1;

            if (other.CompareTag("Vomit"))
            {
                Vomit vomit = other.gameObject.GetComponent<Vomit>();
                vomit.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(vomit.GetDamage() - (.25f * baseDefense)) + 1;
            }
            else if (other.CompareTag("Disk"))
            {
                LoteusDisk disk = other.gameObject.GetComponent<LoteusDisk>();
                disk.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(disk.GetDamage() - (.25f * baseDefense)) + 1;
            }
            else if (other.CompareTag("LewgsAttack"))
            {
                LewgsAttack lewgsA = other.gameObject.GetComponent<LewgsAttack>();
                lewgsA.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(lewgsA.GetDamage() - (.25f * baseDefense)) + 1;
            }
            else if (other.CompareTag("TeegBullet"))
            {
                TeegBullet bullet = other.gameObject.GetComponent<TeegBullet>();
                bullet.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(bullet.GetDamage() - (.25f * baseDefense)) + 1;
            }
            else if (other.CompareTag("GeejusVomit"))
            {
                GeejusVommit gVomit = other.gameObject.GetComponent<GeejusVommit>();
                gVomit.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(gVomit.GetDamage() - (.25f * baseDefense)) + 1;
            }
            else if (other.CompareTag("bigGeejVomit"))
            {
                BigGeejVomit bGVomit = other.gameObject.GetComponent<BigGeejVomit>();
                bGVomit.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(bGVomit.GetDamage() - (.25f * baseDefense)) + 1;
                bGVomit.healOnDamage();
            }
            else if (other.CompareTag("BigGeejBullet"))
            {
                BigGeejBullet bGBullet = other.gameObject.GetComponent<BigGeejBullet>();
                bGBullet.onPlayerDamage(); //turns off damageBox for 1 second
                damage = (int)(bGBullet.GetDamage() - (.25f * baseDefense)) + 1;
                bGBullet.healOnDamage();
            }
            else
            {
                damage = 0;
            }
            
            if (damage < 1)
            {
                damage = 1;
            }

            currHealth -= damage;
            healthBar.value = currHealth;
            if (healthBar.value <= 0)
            {
                GameReset();
                uic.MenuLoad();
            }
        }
    }

    // For physics calculations
    void FixedUpdate()
    {
        Move();
    }

    public void GameReset()
    {
        baseHealth = 80;
        baseAttack = 18;
        maxDefense = 96;
        baseDefense = 32;
        baseSpeed = 5;
        currHealth = baseHealth;
        lifeSteal = 1;
        dashCooldown = 5;
        buffCooldown = 25;
        buffDuration = 10;
        buffAttack = 3;
        buffDefense = 4;
        buffSpeed = 1;
        dashSpeed = 15;
        numLvlsCompleted = 0;
        uic.ResetLevels();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if ((moveX == 0 && moveY == 0) && moveDirection.x != 0 || moveDirection.y != 0)
        {
            lastMoveDirection = moveDirection;
        }

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            //Instantiate(hitbox, finalVector, quaternion.identity);
            //StartCoroutine("MouseHitBoxTimer");
            StartCoroutine(AttackAnimTimer());
        }
        if (Input.GetMouseButtonDown(1) && canDash)//dash input (right click)
        {
            //Dash();
            initDashPos = transform.position;
            dashTarget = cam.ScreenToWorldPoint(Input.mousePosition);
            if (dashTarget.x > transform.position.x)
            {
                lastMoveDirection.x = 1;
            }
            else if (dashTarget.x < transform.position.x)
            {
                lastMoveDirection.x = -1;
            }
            StartCoroutine(Dash());
            StartCoroutine(DashTimer());
        }
        if (Input.GetKeyDown(KeyCode.E) && canBuff)//buff input ("E")
        {
            StartCoroutine(BuffTimer());
            StartCoroutine(Buff());
            StartCoroutine(BuffAnim());
        }

        if (Input.GetKeyDown(KeyCode.Q) && canLS)//activate lifesteal
        {
            StartCoroutine(Lifesteal());
            StartCoroutine(LSCooldownTimer());
        }
    }

    private IEnumerator Lifesteal()//activates lifesteal for 5 seconds
    {
        lSActive = true;
        yield return new WaitForSeconds(5);
        lSActive = false;
    }

    private IEnumerator LSCooldownTimer()
    {
        canLS = false;
        lSRemainingCD = lSCooldown;
        for (int i = 1; i <= lSCooldown; i++)
        {
            yield return new WaitForSeconds(1);
            lSRemainingCD = lSCooldown - i;
        }
        canLS = true;
    }

    public bool GetLSActive()
    {
        return lSActive;
    }

    public float GetLSPercent()
    {
        return lSPercent;
    }

    private IEnumerator BuffAnim()
    {
        isBuffing = true;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isBuffing = false;
            }
            yield return new WaitForSeconds(.5f);
        }
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                isDashing = false;
            }
            yield return new WaitForSeconds(.3f);
        }
    }

    private IEnumerator HitBoxTimer(BoxCollider2D hitBox)
    {
        SpriteRenderer hitBox_sprite = hitBox.gameObject.GetComponent<SpriteRenderer>();
        hitBox_sprite.enabled = true;
        isAttacking = true;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                hitBox.enabled = false;
                hitBox_sprite.enabled = false;
                isAttacking = false;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator AttackAnimTimer()
    {
        isPreAttack = true;
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 mouseToPlayer = mousePosition - transform.position;
        attackVector = mouseToPlayer.normalized;
        attackAngle = (Mathf.Atan2(mouseToPlayer.y, mouseToPlayer.x) * Mathf.Rad2Deg);
        if (attackAngle < 90 && attackAngle > -90)
        {
            lastMoveDirection.x = 1;
        }
        else
        {
            lastMoveDirection.x = -1;
        }

        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                StartCoroutine(MouseHitBoxTimer());
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator DashTimer()
    {
        canDash = false;
        dashRemainingCD = dashCooldown;
        for (int i = 1; i <= dashCooldown; i++)
        {
            yield return new WaitForSeconds(1);
            dashRemainingCD = dashCooldown - i;
        }
        canDash = true;
    }

    private IEnumerator BuffTimer()
    {
        canBuff = false;
        buffRemainingCD = buffCooldown;
        for (int i = 1; i <= buffCooldown; i++)
        {
            yield return new WaitForSeconds(1);
            buffRemainingCD = buffCooldown - i;
        }
        canBuff = true;
    }

    private IEnumerator Buff()
    {
        baseDefense += buffDefense;
        baseAttack += buffAttack;
        baseSpeed += buffSpeed;
        isBuffed = true;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                baseDefense -= buffDefense;
                baseAttack -= buffAttack;
                baseSpeed -= buffSpeed;
                isBuffed = false;
            }

            yield return new WaitForSeconds(buffDuration);
        }
    }

    private IEnumerator MouseHitBoxTimer()
    {
        finalVector = transform.position + attackVector;
        finalVector.y = finalVector.y - .175f;//corrects the location of the vine spawning
        hitbox.transform.rotation = Quaternion.AngleAxis(attackAngle, Vector3.forward);
        hitbox.transform.position = finalVector;
        
        SpriteRenderer hitbox_sprite = hitbox.gameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D hitbox_collider = hitbox.GetComponent<BoxCollider2D>();
        Animator hitbox_anim = hitbox.GetComponent<Animator>();
        hitbox_collider.enabled = true;
        hitbox_sprite.enabled = true;
        isAttacking = true;
        hitbox_anim.SetBool("isAttacking", isAttacking);
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                hitbox_collider.enabled = false;
                hitbox_sprite.enabled = false;
                isAttacking = false;
                isPreAttack = false;
                hitbox_anim.SetBool("isAttacking", isAttacking);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    void Move()
    {
        if (isDashing)
        {
            //transform.position = Vector3.Lerp(initDashPos, dashTarget, dashSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
        }
        else if (!isPreAttack && !isBuffing)
        {
            rb.velocity = new Vector2(moveDirection.x * baseSpeed, moveDirection.y * baseSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0,0);
        }
    }

    // Setting up values for animation
    void Animate()
    {
        anim.SetFloat("AnimMoveX", moveDirection.x);
        anim.SetFloat("AnimMoveY", moveDirection.y);
        anim.SetFloat("AnimMoveMagnitude", moveDirection.magnitude);
        anim.SetFloat("AnimLastMoveX", lastMoveDirection.x);
        anim.SetFloat("AnimLastMoveY", lastMoveDirection.y);
        anim.SetBool("AnimAttack", isPreAttack);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isBuffing", isBuffing);
    }

    //Base Health
    public int GetBaseHealth()
    {
        return baseHealth;
    }
    public void SetBaseHealth(int newHealth)
    {
        baseHealth = newHealth;
    }

    //Base Attack
    public int GetBaseAttack()
    {
        return baseAttack;
    }
    public void SetBaseAttack(int newAttack)
    {
        baseAttack = newAttack;
    }

    //Base Defence
    public int GetBaseDefense()
    {
        return baseDefense;
    }
    public void SetBaseDefense(int newDefense)
    {
        baseDefense = newDefense;
    }

    //Base Speed
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }
    public void SetBaseSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
    }

    //Current Health
    public int GetCurrHealth()
    {
        return currHealth;
    }
    public void SetCurrHealth(int newCurrHealth)
    {
        currHealth = newCurrHealth;
    }

    public int GetLifeSteal()
    {
        return lifeSteal;
    }
    public void SetLifeSteal(int newLifeSteal)
    {
        lifeSteal = newLifeSteal;
    }

    public float GetLSCD()
    {
        return lSCooldown;
    }

    public void SetLSCD(float newLSCD)
    {
        lSCooldown = newLSCD;
    } 
    
    public int GetBuffCD()
    {
        return buffCooldown;
    }

    public void SetBuffCD(int newBuffCD)
    {
        buffCooldown = newBuffCD;
    }

    public int GetBuffBA()
    {
        return buffAttack;
    }

    public void SetBuffBA(int newBuffAttack)
    {
        buffAttack = newBuffAttack;
    }
    public int GetBuffBD()
    {
        return buffDefense;
    }

    public void SetBuffBD(int newBuffDefense)
    {
        buffDefense = newBuffDefense;
    }
    
    public float GetBuffBS()
    {
        return buffSpeed;
    }
    
    public void SetBuffBS(float newBuffSpeed)
    {
        buffSpeed = newBuffSpeed;
    }
    
    public void IncNumLvlsCompleted()
    {
        numLvlsCompleted += 1;
    }
    public int GetNumLvlsCompleted()
    {
        return numLvlsCompleted;
    }
    public void ResetBuff()
    {
        StopCoroutine(Buff());
        baseDefense -= buffDefense;
        baseAttack -= buffAttack;
        baseSpeed -= buffSpeed;
    }
    public void SetIsBuffed(bool newVal)
    {
        isBuffed = newVal;
    }
    public bool GetIsBuffed()
    {
        return isBuffed;
    }

    public void IncSpeed()
    {
        baseSpeed += .6f;
    }

    public void DecSpeed()
    {
        baseSpeed -= .6f;
    }

    public void IncAttack()
    {
        baseAttack += 2;
    }
    public void DecAttack()
    {
        baseAttack -= 2;
    }
    
    public void IncDefense()
    {
        baseDefense += 4;
    }

    public void DecDefense()
    {
        baseDefense -= 4;
    }
}
