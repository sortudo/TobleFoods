using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  Script responsible to control Button actions
public class ButtonManager : MonoBehaviour
{
    public Text text;

    // SetAlign Functions are responsible to align the text with the button' sprite, if they are pressed or not
    public void setAligntoLower()
    {
        text.alignment = TextAnchor.LowerCenter;
    }

    public void setAligntoMiddle()
    {
        text.alignment = TextAnchor.MiddleCenter;
    }

    // Function that prohibit Hints to pop-up when they should not
    // NoHints and YesHints are used to prevent hints during pause 
    public void NoHints()
    {
        if (BoardManager.instance.pm != null)
            BoardManager.instance.pm.DyingLight();
        BoardManager.instance.StopAllCoroutines();
    }

    // Function that allow Hints to pop-up
    public void YesHints()
    {
        BoardManager.instance.StartCoroutine(BoardManager.instance.pm.Coroutine_HighlightIt());
    }

    // Function that plays background music
    public void PlayMusic()
    {
        Music.instance.music.pitch = 1.0f;
        Music.instance.music.Play();
    }
}