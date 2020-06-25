using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Script responsible to place the progress bar right above the board game for every kind of portrait or resolution
public class Align_progressbar : MonoBehaviour
{
    private RectTransform bar_r;

    void Start()
    {
        bar_r = this.GetComponent<RectTransform>();

        UIManager.instance.ScreenSizeChangeEvent += Align_Progress;
        Align_Progress();
    }

    public void Align_Progress()
    {
        bar_r.anchoredPosition = new Vector2(0,(bar_r.rect.height / 2)) + Board_side.instance.getPosition(0, -4);
        bar_r.sizeDelta = new Vector2(Board_side.instance.size, bar_r.rect.height);
    }
}
