using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeegBullet : MonoBehaviour
{
    [SerializeField] private BoxCollider2D damageBox;

    private float waitTime = .5f;
    private Teeg _teeg;
    private GameObject playerObj;
    private Rigidbody2D rb;
    private float origonalX;
    private float origonalY;

    private float currX;
    private float lastXPos;
    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        origonalX = playerObj.transform.position.x;
        origonalY = playerObj.transform.position.y;
        _teeg = GetComponentInParent<Teeg>();
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(DiskDespawnTimer());
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }

    public void onPlayerDamage()
    {
        //TODO: possibly remove disk on player contact
        Debug.Log("test here");
        Destroy(this.gameObject);
    }

    public int GetDamage()
    {
        return _teeg.GetAttack();
    }

    private void Move()
    {
        float xVal = origonalX - rb.transform.position.x;
        float yVal = origonalY - rb.transform.position.y;
        Vector2 dir = new Vector2(xVal,yVal).normalized * 300;
        rb.AddForce(dir);
    }
    
    private IEnumerator DiskDespawnTimer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(5);
        }
    }
}
