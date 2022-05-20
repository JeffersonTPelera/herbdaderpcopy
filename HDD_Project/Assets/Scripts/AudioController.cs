using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        SwitchAudioClip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Switch music depending on the scene
     public void SwitchAudioClip ()
     { 
         Scene currentScene = SceneManager.GetActiveScene ();
         string targetLevel = currentScene.name;
         
         if (targetLevel == "MainMenu"||targetLevel =="Controls"||targetLevel=="Settings")
         {
             audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-TT") as AudioClip;
         }

         //Music for the easy levels
         else if (targetLevel.StartsWith("Game 1"))
         {
             RandomlyPickMusicLevel();
         }
     
         //Music for the normal levels
         else if (targetLevel.StartsWith("Game 2"))
         {
             audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-56") as AudioClip;
         }
         
         //Music for the hard levels
         else if (targetLevel.StartsWith("Game 3"))
         {
             audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-01") as AudioClip;
         }
         
         //Music for the boss level
         else if (targetLevel.StartsWith("Game 4"))
         {
             audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NAVAO-40") as AudioClip;
         }
         
         else if (targetLevel.StartsWith("Game 5"))
         {
             audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-SR") as AudioClip;
         }
         
         audioSource.Play();
     }
    //Function to randomly pick music in normal level
    public void RandomlyPickMusicLevel()
    {
        //Picks either 1 or 2
        int randomIndex = Random.Range(1, 3);

        //The switch cases for which mp3 file to play
        switch (randomIndex)
        {
            case 1:
                audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-24") as AudioClip;
                break;
            case 2:
                audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-03") as AudioClip;
                break;
            default:
                audioSource.clip = Resources.Load<AudioClip>("Sounds/MusMus-CT-NV-24") as AudioClip;
                break;
        }
    }
}
