using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
