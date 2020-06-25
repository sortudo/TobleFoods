using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Script responsible to control each TobleFood through the gameplay
public class TobleGem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TobleSO TobleSO;
    public int x;
    public int y;
    public Vector2 pos;

    private SpriteRenderer artwork;
    private RectTransform Gem_r;
    private Image Gem_i;
    private bool updating;

    private void Start()
    {
        Gem_r = GetComponent<RectTransform>();
        Gem_i = GetComponent<Image>();

        SetGemCore();

        Gem_r.anchoredPosition = new Vector2(0, 0);
        
        UIManager.instance.ScreenSizeChangeEvent += Align_Gem;
        Align_Gem();

    }

    // Function that align the TobleFood according to the current resolution
    public void Align_Gem()
    {
        Gem_r.anchoredPosition = ResetPosition();
        Gem_r.sizeDelta = Board_side.instance.getPosition(1, -1);
    }

    // Function that updates the TobleFood real position in the game and return it
    public Vector2 ResetPosition()
    {
        pos = new Vector2(Board_side.instance.size / 16, Board_side.instance.size / 16) + Board_side.instance.getPosition(x, y);
        return pos;
    }

    // Function that update its Sprite and tag
    public void SetGemCore()
    {
        this.gameObject.tag = TobleSO.tag;
        Gem_i.sprite = TobleSO.artwork;
    }

    // Function that checks if it is moving or not
    public bool UpdateGem()
    {
        if(Vector3.Distance(Gem_r.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            MovePositionTo(pos);
            updating = false;
            return false;
        }
    }

    // Function that moves at a set direction
    public void MovePosition(Vector2 move)
    {
        Gem_r.anchoredPosition += move * Time.deltaTime * Board_side.instance.size / 64;
    }

    // Function that move to a set point
    public void MovePositionTo(Vector2 move)
    {
        Gem_r.anchoredPosition = Vector2.Lerp(Gem_r.anchoredPosition, move, Time.deltaTime * Board_side.instance.size / 64);
    }
    
    // If the player clicks at it
    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        transform.SetAsLastSibling();
        MoveGem.instance.Move(this);
    }

    // If the player releases the click
    public void OnPointerUp(PointerEventData eventData)
    {
        MoveGem.instance.Drop();
        transform.SetAsFirstSibling();
    }

}
