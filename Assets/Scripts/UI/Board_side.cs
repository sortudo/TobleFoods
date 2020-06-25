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

        
        UIManager.instance.ScreenSizeChangeEvent += CalculateSize;
        CalculateSize();
    }

    // Function that calculate the size
    public void CalculateSize()
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

    // Function that calculate the real position at the board
    public Vector2 getPosition(int x, int y)
    {
        return new Vector2(x * size / 8, -y * size / 8);
    }
}
