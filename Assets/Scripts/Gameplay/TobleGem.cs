using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Script responsible to control each TobleFood through the gameplay
public class TobleGem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // TobleFood Gem
    public TobleSO TobleSO;
    
    // TobleFood components
    public RectTransform Gem_r;
    public Animator Gem_a;
    public Outline Gem_o;
    public ParticleSystem Gem_p;
    public Image Gem_i;

    // TobleFood's Position and movement
    public int x;
    public int y;
    public bool updating;
    public Vector2 pos;

    private void Start()
    {
        Gem_r = GetComponent<RectTransform>();
        Gem_i = GetComponent<Image>();
        Gem_a = GetComponent<Animator>();
        Gem_o = GetComponent<Outline>();
        Gem_p = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        
        UpdateInterface();

        Gem_o.enabled = false;
        Gem_r.anchoredPosition = new Vector2(0, 0);
        
        UIManager.instance.ScreenSizeChangeEvent += Align_Gem;
        Align_Gem();

    }

    // Function that update SO, sprite and color's outline
    public void UpdateInterface()
    {
        this.gameObject.tag = TobleSO.tag;
        Gem_i.sprite = TobleSO.artwork;
        Gem_o.effectColor = TobleSO.color;

        var main = Gem_p.main;
        main.startColor = TobleSO.color;
        Gem_p.gameObject.SetActive(false);
    }

    // Function that Destroy this TobleFood
    public void DestroyGem()
    {
        Gem_a.SetBool("DestroyIt", false);
        Gem_i.enabled = true;
        Gem_o.enabled = false;

        // Check if all TobleFoods from the matching list were destroyed correctly
        BoardManager.instance.destroyed.RemoveAt(0);
        if(BoardManager.instance.destroyed.Count == 0)
        {
            BoardManager.instance.ApplyGravityToTobleFood();
            StatusManager.instance.UpdateScore(BoardManager.instance.points_move);
            BoardManager.instance.points_move = 0;
            BoardManager.instance.destroyed = new List<TobleGem>();
        }    
    }

    // Function that align the TobleFood according to the current resolution and position
    public void Align_Gem()
    {
        Gem_r.anchoredPosition = ResetPosition();
        Gem_r.sizeDelta = Board_side.instance.getPosition(1, -1);

        var main = Gem_p.main;
        main.startSize = 1.0f * Board_side.instance.size/ 2048;
        main.startSpeed = 1.0f * Board_side.instance.size/ 256;
    }

    // Function that updates the TobleFood real position in the game and return it
    public Vector2 ResetPosition()
    {
        pos = new Vector2(Board_side.instance.size / 16, Board_side.instance.size / 16) + Board_side.instance.getPosition(x, y);
        return pos;
    }

    // Function that checks if it is moving or not
    public bool UpdateGem()
    {
        ResetPosition();
        if (Vector3.Distance(Gem_r.anchoredPosition, pos) > 1)
        {
            //Moving
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            // Not Moving
            Align_Gem();
            updating = false;
            return false;
        }
        
    }

    // Function that move to a set point
    public void MovePositionTo(Vector2 move)
    {
        Gem_r.anchoredPosition = Vector2.Lerp(Gem_r.anchoredPosition, move, Time.deltaTime * Board_side.instance.size / 128);
    }
    
    // If the player clicks at it
    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating || gameObject.tag == "TobleDestroyed") return;

        Gem_i.color = new Color(.5f, .5f, .5f, 1.0f);
        transform.SetAsLastSibling();
        MoveGem.instance.Move(this);

        SFXManager.instance.PlaySFX(TobleSO.Select);
    }

    // If the player releases the click
    public void OnPointerUp(PointerEventData eventData)
    {
        Gem_i.color = Color.white;
        MoveGem.instance.Drop();
        transform.SetAsFirstSibling();
    }

}
