using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCanvas : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        disableChildren();
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
}
