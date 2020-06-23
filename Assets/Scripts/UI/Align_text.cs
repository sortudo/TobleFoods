using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  Script responsible to align the text with the button if the latter is pressed or not
public class Align_text : MonoBehaviour
{
    public Text text;

    public void setAligntoLower()
    {
        text.alignment = TextAnchor.LowerCenter;
    }

    public void setAligntoMiddle()
    {
        text.alignment = TextAnchor.MiddleCenter;
    }
}
