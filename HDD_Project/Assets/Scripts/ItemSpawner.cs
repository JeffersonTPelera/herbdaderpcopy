using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject attackItem, defenseItem, healthItem, speedBuff;
    //[SerializeField] private int numItems;
    private int check;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*Function to spawn item
    Call it using something like spawnItem.SpawnItem(this.gameObject.transform.position.x,this.gameObject.transform.position.y);
    if the enemy's current health is less than 0
    */
    public void SpawnItem(float spawnX, float spawnY)
    {
        Vector2 itemPosition = new Vector2(spawnX, spawnY);

        //Randomly decide which item to spawn
        check = Random.Range(0, 4);
        switch (check)
        {
            case 0:
                Instantiate(attackItem, itemPosition, Quaternion.identity);
                break;
            case 1:
                Instantiate(defenseItem, itemPosition, Quaternion.identity);
                break;
            case 2:
                Instantiate(healthItem, itemPosition, Quaternion.identity);
                break;
            case 3:
                Instantiate(speedBuff, itemPosition, Quaternion.identity);
                break;
            default:
                Instantiate(healthItem, itemPosition, Quaternion.identity);
                break;
        }
    }
}
