using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to play the SFX Sounds
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance = null;
    public AudioSource sfx;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        instance = GetComponent<SFXManager>();
        sfx = this.GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.Stop();
        sfx.PlayOneShot(clip);  
    }
}
