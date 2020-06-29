using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Script responsible to show the HighScore int the Start Menu
public class GetHighScore : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Text>().text = PlayerPrefs.GetInt("HighScore").ToString();
    }
}
