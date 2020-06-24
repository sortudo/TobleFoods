using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton responsible to calculate the size of the game board' side on a current resolution
public class Board_side : MonoBehaviour
{
    public RectTransform board_r;
    public float size;
    public static Board_side instance = null;

    private float width;
    private float height;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    void Update()
    {
        width = board_r.rect.width;
        height = board_r.rect.height;

        if (width < height)
        {
            size = width;
        }
        else
        {
            size = height;
        }
    }
}
