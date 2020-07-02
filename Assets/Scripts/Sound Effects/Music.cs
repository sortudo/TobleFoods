using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to control the Background Music
public class Music : MonoBehaviour
{
    public static Music instance = null;

    public AudioSource music;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        instance = GetComponent<Music>();
        music = this.GetComponent<AudioSource>();
    }
}
