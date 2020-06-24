using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to control each TobleFood through the gameplay
public class TobleGem : MonoBehaviour
{
    public TobleSO TobleSO;
    public int x;
    public int y;

    private SpriteRenderer artwork;
    private RectTransform Gem_r;

    private void Start()
    {
        Gem_r = transform.GetComponent<RectTransform>();
        Gem_r.anchoredPosition = new Vector2(0, 0);
    }

    private void Update()
    {
        TobleSO.Align_Gem(x, y, Gem_r, Board_side.instance.size);
    }
}
