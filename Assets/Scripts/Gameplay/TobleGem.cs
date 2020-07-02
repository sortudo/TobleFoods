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

    private void Awake()
    {
        Gem_a = GetComponent<Animator>();
        Gem_o = GetComponent<Outline>();
    }

    private void Start()
    {
        Gem_r = GetComponent<RectTransform>();
        Gem_i = GetComponent<Image>();
        Gem_p = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        
        UpdateInterface();

        pos = new Vector2(0, 0);

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

        BoardManager.instance.DestroyGemOnBoard();
    }

    // Function that will be called in the end of Disappear animation, destroying this TobleFood
    public void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }

    // Function that align the TobleFood according to the current resolution and position
    public void Align_Gem()
    {
        Gem_r.anchoredPosition = ResetPosition();
        Gem_r.sizeDelta = Board_side.instance.getPosition(1, -1);

        // Sets size and speed of the particle
        var main = Gem_p.main;
        main.startSize = 1.0f * Board_side.instance.size/ 2048;
        main.startSpeed = 1.0f * Board_side.instance.size/ 256;
    }

    // Function that updates the TobleFood pos without updating the Rect Transform
    public Vector2 ResetPosition()
    {
        pos = new Vector2(Board_side.instance.size / 16, Board_side.instance.size / 16) + Board_side.instance.getPosition(x, y);
        return pos;
    }

    // Function that checks if this TobleFood is moving or not
    // Return a true if still needs to update or false if does not
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

    // Function that move to a point
    public void MovePositionTo(Vector2 move)
    {
        Gem_r.anchoredPosition = Vector2.Lerp(Gem_r.anchoredPosition, move, Time.deltaTime * Board_side.instance.size / 128);
    }
    
    // If the player clicks at it
    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating || gameObject.tag == "TobleDestroyed") return;

        Gem_i.color = new Color(.5f, .5f, .5f, 1.0f); // Color of pressed TobleFood
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
