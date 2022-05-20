using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vomit : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D damageBox;

    private float waitTime = .3f;
    private Geej _geej;
    // Start is called before the first frame update
    void Start()
    {
        _geej = GameObject.FindGameObjectWithTag("Geej").GetComponent<Geej>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void onPlayerDamage()
    {
        Debug.Log("test here");
        damageBox.enabled = false;
        StartCoroutine("damageBoxTimer");
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
    
    public int GetDamage()
    {
        return _geej.GetAttack();
    }
}
