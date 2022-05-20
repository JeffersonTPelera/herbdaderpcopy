using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject player;

    private EnemySpawner _enemySpawner;
    private int numEnemies;
    private int width, height;
    
    //private int count = 0;
    private UIController _uiController;
    private CardController cards;
    private CardBuffs cardBuffs;

    List<GameObject> children = new List<GameObject>();
    
    //For the pause menu UI
    [Header("Pause Menu UI")]
    [SerializeField] private Image pauseMenuBackground;
    [SerializeField] private Text pauseMenuText;
    [SerializeField] private Button controlsButton, settingsButton, closeButton, exitButton, nextLevelButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _uiController = FindObjectOfType<UIController>();
        ClosePauseMenu();
        _enemySpawner = gameObject.GetComponentInChildren<EnemySpawner>();
        numEnemies = _enemySpawner.GetNumEnemies();
        
        GameObject cardsObj =  GameObject.Find("Card Buffs");
        cards = cardsObj.GetComponent<CardController>();
        cardBuffs = GameObject.Find("Card Buffs").GetComponentInChildren<CardBuffs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetKeyDown("p"))
        {
            OpenPauseMenu();
        }

        numEnemies = _enemySpawner.GetNumEnemies();
        if (numEnemies == 0)
        {
            StartCoroutine(OfferNextLevel());
        }
    }

    
    //The function when player opens P to pause menu
    private void OpenPauseMenu()
    {
        Time.timeScale = 0f; //Freezes the game in the background
        
        //Set the pause menu objects active
        pauseMenuBackground.gameObject.SetActive(true);
        pauseMenuText.gameObject.SetActive(true);
        controlsButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true); 
        exitButton.gameObject.SetActive(true);
    }

    //Function to close the pause menu
    public void ClosePauseMenu()
    {
        Time.timeScale = 1; //Unfreezes the game
        
        //Sets the pause menu objects unactive
        pauseMenuBackground.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
        controlsButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false); 
        exitButton.gameObject.SetActive(false);
    }
    
    //The function to generate the level's tiles
    public void GenerateTiles()
    {
        width = 7;
        height = 7;
        for (int i = 0; i != width; ++i)
        {
            for (int j = 0; j != height; j++)
            {
                var map = new Vector2(i, j);
                Instantiate(tile, map, Quaternion.identity);
            }
        }
        
    }

    private Collider2D[] ObjExistsAt(float x, float y)
    {
        //Collider2D[] arr1 = Physics2D.OverlapCircleAll(new Vector3(x, y-.1f, 0), .01f);
        Collider2D[] arr2 = Physics2D.OverlapCircleAll(new Vector3(x, y+.1f, 0), .01f);
        //IEnumerable<Collider2D> toReturn = arr1.Intersect(arr2);
        //return toReturn.ToArray();
        return arr2;
    }

    IEnumerator OfferNextLevel()
    {
        yield return new WaitForSeconds(5);
        cards.ActiveCards();
    }
    
    public void OfferNextLevelAfter()
    {
        Debug.Log("You reached the end!");
        cards.UnActiveCards(true);
        nextLevelButton.gameObject.SetActive(true);
    }
}
