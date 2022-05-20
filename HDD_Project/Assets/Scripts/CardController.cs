using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    //Variables for the three button
    [SerializeField] private GameObject buttonOne;
    [SerializeField] private GameObject buttonTwo;
    [SerializeField] private GameObject buttonThree;

    private List<GameObject> cardList;
    private GameObject[] cardListArray;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeCards();
        UnActiveCards(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RandomizeCards ()
    {
        //Store all card prefabs into array
        cardListArray = Resources.LoadAll<GameObject>("CardPrefabs");
   
        //Set it to a list
        cardList = cardListArray.ToList();

        for (int i = 0; i < 3; i++)
        {
            GameObject cardBuild = cardList[Random.Range(0, cardList.Count)];
            if (i == 0)
            {
                buttonOne = Instantiate(cardBuild, buttonOne.transform.position, Quaternion.identity);
                buttonOne.transform.parent = GameObject.Find("Card Buffs").transform;
            }

            if (i == 1)
            {
                buttonTwo = Instantiate(cardBuild, buttonTwo.transform.position, Quaternion.identity);
                buttonTwo.transform.parent = GameObject.Find("Card Buffs").transform;
            }
            
            if (i == 2)
            {
                buttonThree = Instantiate(cardBuild, buttonThree.transform.position, Quaternion.identity);
                buttonThree.transform.parent = GameObject.Find("Card Buffs").transform;
            }
        }
    }

    //Set all children objects to not active
    public void UnActiveCards(bool checkClicked)
    {
        Debug.Log("Unactivating cards...");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        
        if(checkClicked) {
            this.gameObject.SetActive(false);   
        }
    }
    
    //Call this function at the end of level
    public void ActiveCards()
    {
        for(int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
