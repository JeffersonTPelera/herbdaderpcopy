using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private int numEnemies;
    [SerializeField] private float xLeftBound;
    [SerializeField] private float xRightBound;
    [SerializeField] private float yTopBound;
    [SerializeField] private float yBottomBound;
    private Vector2[] spawnPos;
    [SerializeField] private BoxCollider2D[] spawnArea;
    [SerializeField] private GameObject[] elites;
    [SerializeField] private int numElites;
    [SerializeField] private bool bossLvl;

    // Start is called before the first frame update
    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        int lvl = player.GetNumLvlsCompleted();
        if (lvl < 3)//stage 1
        {
            for (int i = 0; i < numEnemies;)
            {
                int randNum = Random.Range(0, enemy.Length);
                float randX = Random.Range(xLeftBound, xRightBound);
                float randY = Random.Range(yBottomBound, yTopBound);
                Vector2 enemyPos = new Vector2(randX,randY);
                foreach (var box in spawnArea)
                {
                    if (box.bounds.Contains(enemyPos))
                    {
                        Instantiate(enemy[randNum],enemyPos,Quaternion.identity);
                        i++;
                    }
                }
            }
        }
        else//stages where elites spawn
        {
            for (int i = 0; i < numEnemies;)
            {
                int randNum = Random.Range(0, enemy.Length);
                float randX = Random.Range(xLeftBound, xRightBound);
                float randY = Random.Range(yBottomBound, yTopBound);
                Vector2 enemyPos = new Vector2(randX,randY);
                foreach (var box in spawnArea)
                {
                    if (box.bounds.Contains(enemyPos))
                    {
                        Instantiate(enemy[randNum],enemyPos,Quaternion.identity);
                        i++;
                    }
                }
            }
            for (int i = 0; i < numElites;)
            {
                int randNum = Random.Range(0, elites.Length);
                float randX = Random.Range(xLeftBound, xRightBound);
                float randY = Random.Range(yBottomBound, yTopBound);
                Vector2 enemyPos = new Vector2(randX,randY);
                foreach (var box in spawnArea)
                {
                    if (box.bounds.Contains(enemyPos))
                    {
                        Instantiate(elites[randNum],enemyPos,Quaternion.identity);
                        i++;
                    }
                }
            }
        }
        numEnemies = numElites + numEnemies;//sets the number of things that needs to be killed for next stage
        if (bossLvl)
        {
            numEnemies += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int GetNumEnemies()
    {
        return numEnemies;
    }

    public void SetNumENemies(int newNumEnemies)
    {
        numEnemies = newNumEnemies;
    }
}
