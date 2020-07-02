using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to play the Status Sounds
public class StatusSound : MonoBehaviour
{
    public static StatusSound instance = null;
    public AudioSource status;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        instance = GetComponent<StatusSound>();
        status = this.GetComponent<AudioSource>();
    }

    public void PlayStatus(AudioClip clip)
    {
        status.Stop();
        status.PlayOneShot(clip);
    }
}
