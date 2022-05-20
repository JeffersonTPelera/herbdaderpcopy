using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoteusDisk : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D damageBox;

    private float waitTime = .5f;
    private Loteus _loteus;
    private bool isInitiallyLeft;
    private bool isLeft;
    private Animator disk_anim;
    private GameObject playerObj;
    private Rigidbody2D rb;

    private float currX;
    private float lastXPos;
    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        disk_anim = GetComponent<Animator>();
        _loteus = GetComponentInParent<Loteus>();
        isInitiallyLeft = _loteus.isLeft;
        StartCoroutine(DiskDespawnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        currX = transform.position.x;
        if (currX > lastXPos)//right
        {
            isLeft = false;
        }
        else if (currX < lastXPos)//left
        {
            isLeft = true;
        }
        lastXPos = currX;
        //Move();
        Animate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Animate()
    {
        disk_anim.SetBool("isLeft", isLeft);
    }
    
    public void onPlayerDamage()
    {
        //TODO: possibly remove disk on player contact
        Debug.Log("test here");
        _loteus.SetIsIdle(false);
        Destroy(this.gameObject);
    }

    public int GetDamage()
    {
        return _loteus.GetAttack();
    }

    private void Move()
    {
        float xVal = playerObj.transform.position.x - rb.transform.position.x;
        float yVal = playerObj.transform.position.y - rb.transform.position.y;
        Vector2 dir = new Vector2(xVal,yVal).normalized * 8f;
        rb.AddForce(dir);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity,4f);
    }
    
    private IEnumerator DiskDespawnTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                _loteus.IdleTimerStart();
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(3);
        }
    }
}
