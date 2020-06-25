using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to create an event that triggers functions when the resolution is changed
public class UIManager : MonoBehaviour
{
    public delegate void ScreenSizeChangeEventHandler();       
    public event ScreenSizeChangeEventHandler ScreenSizeChangeEvent;                
    protected virtual void OnScreenSizeChange()
    {             
        if (ScreenSizeChangeEvent != null) ScreenSizeChangeEvent();
    }

    private Vector2 lastScreenSize;
    public static UIManager instance = null;                                   

    void Awake()
    {
        if (instance == null)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    // Check if the resolution changed
    void LateUpdate()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        if (this.lastScreenSize != screenSize)
        {
            this.lastScreenSize = screenSize;
            OnScreenSizeChange();
            
        }
    }
}
