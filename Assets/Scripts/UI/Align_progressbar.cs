using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Script responsible to place the progress bar right above the board game for every kind of portrait or resolution
public class Align_progressbar : MonoBehaviour
{
    private RectTransform board_r;
    private RectTransform bar_r;
    private float width;
    private float height;

    void Start()
    {
        board_r = this.transform.parent.gameObject.GetComponent<RectTransform>();
        bar_r = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        width = board_r.rect.width;
        height = board_r.rect.height;

        // The smallest side of the board's gameobject has always the same size of the board' side
        // And then update anchored position and size delta from the progress bar according to board' side
        if (width < height)
        {
            bar_r.anchoredPosition = new Vector2(board_r.anchoredPosition.x, width/2 + (bar_r.rect.height/2));
            bar_r.sizeDelta = new Vector2(width, bar_r.rect.height);
        }
        else
        {
            bar_r.anchoredPosition = new Vector2(board_r.anchoredPosition.x, height/2 + (bar_r.rect.height / 2));
            bar_r.sizeDelta = new Vector2(height, bar_r.rect.height);
        }
        
    }
}
