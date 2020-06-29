using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Script responsible to control the volume of a mixer
public class ControlVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public string para_name;
    
    public void SetLevel (float slider_value)
    {
        mixer.SetFloat(para_name, Mathf.Log10(slider_value) * 20);
    }
}
