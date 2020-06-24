using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable Object that sets all the attributes and methods of a TobleFood
[CreateAssetMenu(fileName = "New TobleFood", menuName = "TobleFood ")]
public class TobleSO : ScriptableObject
{
    public Sprite artwork;
    public int points;

    // Function responsible to align the TobleFood according to its position at the board
    public void Align_Gem(int x, int y, RectTransform Gem_r, float size)
    {
        Gem_r.anchoredPosition = new Vector2(size/16 + (x * size / 8), size/16 - (y * size / 8));
    }
}
