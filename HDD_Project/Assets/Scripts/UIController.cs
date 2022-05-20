using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//The script for the UI, including the menus and game's UI
//Hopefully my group members will read my comments just in case

public class UIController : MonoBehaviour
{
    private static int level = 0;
    static String lastScene = "MainMenu";
    private GameObject player;
    private Player playerScript;
    private ControlsCanvas controlsCanvas;
    private Settings settingsCanvas;
    private ItemController ic;
    
    private static String[] levelsS1 = new String[] //Stage 1 levels
        {
            "Game 1.0", 
            "Game 1.1",
            "Game 1.2",
            "Game 1.3",
            "Game 1.4",
            "Game 1.5",
        };
    private static String[] allS1Levels = levelsS1;//holds all of the stage 1 levels so on restart the levels are put back into the array
    private static String[] levelsS2 = new String[] 
    {
        "Game 2.0", 
        "Game 2.1",
        "Game 2.2",
        "Game 2.3",
        "Game 2.4",
        "Game 2.5"
    };
    private static String[] allS2Levels = levelsS2;//holds all of the stage 1 levels so on restart the levels are put back into the array
    private static String[] levelsS3 = new String[] 
    {
        "Game 3.0", 
        "Game 3.1",
        "Game 3.2",
        "Game 3.3",
        "Game 3.4",
        "Game 3.5"
    };
    private static String[] allS3Levels = levelsS3;//holds all of the stage 1 levels so on restart the levels are put back into the array
    private static String[] levelsS4 = new String[] //boss stages
    {
        "Game 4.0"
    };
    private static String[] allS4Levels = levelsS4;//holds all of the stage 1 levels so on restart the levels are put back into the array

    // Start is called before the first frame update
    void Start()
    {
        //lastScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");
        controlsCanvas = GameObject.FindGameObjectWithTag("Controls Canvas").GetComponent<ControlsCanvas>();
        settingsCanvas = GameObject.FindGameObjectWithTag("Settings Canvas").GetComponent<Settings>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Function to load the level's scence
    public void PlayLoad()
    {
        level += 1;
        if (level != 1)
        {
            playerScript = player.GetComponent<Player>();
            playerScript.IncNumLvlsCompleted();
            if (playerScript.GetIsBuffed())
            {
                playerScript.SetIsBuffed(false);
                playerScript.ResetBuff();
            }
            GameObject[] temp = GameObject.FindGameObjectsWithTag("ItemContoller");
            foreach (var item in temp)
            {
                ic = item.GetComponent<ItemController>();
                ic.EndOfLvlCleanup();
            }
        }

        if (level < 4)
        {
            int rand = Random.Range(0, levelsS1.Length);
            SceneManager.LoadScene(levelsS1[rand]);//loads random game scene
            lastScene = levelsS1[rand];
            levelsS1 = levelsS1.Where((item, index) => index != rand).ToArray();//deletes the already used game scene from the array
        }
        else if (level < 7)
        {
            int rand = Random.Range(0, levelsS2.Length);
            SceneManager.LoadScene(levelsS2[rand]);//loads random game scene
            lastScene = levelsS2[rand];
            levelsS2 = levelsS2.Where((item, index) => index != rand).ToArray();//deletes the already used game scene from the array
        }
        else if (level < 10)
        {
            int rand = Random.Range(0, levelsS3.Length);
            SceneManager.LoadScene(levelsS3[rand]);//loads random game scene
            lastScene = levelsS3[rand];
            levelsS3 = levelsS3.Where((item, index) => index != rand).ToArray();//deletes the already used game scene from the array
        }
        else if (level == 10)
        {
            int rand = Random.Range(0, levelsS4.Length);
            SceneManager.LoadScene(levelsS4[rand]);//loads random game scene
            lastScene = levelsS4[rand];
            levelsS4 = levelsS4.Where((item, index) => index != rand).ToArray();//deletes the already used game scene from the array
        }
        else if(level == 11)
        {
            ResetLevels();
            playerScript.GameReset();
            YouWin();
        }
    }
    
    //Functions for when we have a controls and settings scene
    private void YouWin()
    {
        Debug.Log("You Win!");
    }
    public void ControlsLoad()
    {
        //SceneManager.LoadScene("Controls");//Changes to the controls scene.
        controlsCanvas.enableChildren();
    }
    public void MenuLoad()
    {
        SceneManager.LoadScene("MainMenu");//goes back to main menu.
        lastScene = "MainMenu";
    }
    public void SettingsLoad()
    {
        //SceneManager.LoadScene("Settings"); //loads setting scene;
        settingsCanvas.enableChildren();
    }

    public void BackLoad()
    {
        Debug.Log("lastScene: " + lastScene);
        SceneManager.LoadScene(lastScene);
        /*if (lastScene == "MainMenu")
        {
            SceneManager.LoadScene(lastScene); //loads setting scene;
        }
        else
        {
            Debug.Log("lastScene: " + lastScene);
            SceneManager.LoadScene(lastScene);
        }*/
    }
    
    //Function to quit game
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetLastScene(string newLastScene)
    {
        lastScene = newLastScene;
    }

    public string GetLastScene()
    {
        return lastScene;
    }

    public void ResetLevels()
    {
        levelsS1 = allS1Levels;
        levelsS2 = allS2Levels;
        levelsS3 = allS3Levels;
        levelsS4 = allS4Levels;
        level = 0;
    }
}
