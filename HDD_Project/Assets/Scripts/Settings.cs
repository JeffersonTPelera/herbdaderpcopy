using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for just the settings menu
public class Settings : MonoBehaviour
{
    private bool toggle;
    [SerializeField] private Text onSwitch;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    
    public void disableChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    //Function
    public void StopAllAudio()
    {
        toggle = !toggle;

        if (toggle) {
            AudioListener.volume = 1f;
            onSwitch.text = "ON";
        }
        
        else
        { 
            AudioListener.volume = 0f;
            onSwitch.text = "OFF";
        }
    }
}
