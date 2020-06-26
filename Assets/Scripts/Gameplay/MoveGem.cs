using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to move gems when the user click and slide a gem
public class MoveGem : MonoBehaviour
{
    public static MoveGem instance = null;

    private TobleGem moving;
    private Vector2 mouseStart;
    private int x_i;
    private int y_i;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        // When there is a moving gem
        if(moving != null)
        {
            // Calculate de position of the mouse
            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 ndir = dir.normalized;
            Vector2 adir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            // Four directions that you can move a toblefood
            int x = 0;
            int y = 0;
            if(dir.magnitude > Board_side.instance.size / 64)
            {
                if(adir.x > adir.y)
                {
                    x = (ndir.x > 0) ? 1 : -1;
                    y = 0;
                }else if (adir.y > adir.x)
                {
                    x = 0;
                    y = (ndir.y > 0) ? -1 : 1;
                }
            }
            x_i = x + moving.x;
            y_i = y + moving.y;

            // Move the TobleFood at desired and possible position 
            Vector2 pos = moving.pos;
            if (!(x == 0 && y == 0) && outBoard(x + moving.x, y + moving.y))
                pos +=  new Vector2 (x * Board_side.instance.size/16, - y * Board_side.instance.size / 16);
            moving.MovePositionTo(pos);
        }
    }

    // Function that evaluate if it's a possible coordinate for the board
    public bool outBoard(int x, int y)
    {
        return ! (x > 7) &&
                !(x < 0) &&
                !(y > 7) &&
                !(y < 0);
    }

    // Function that allows a TobleFood to move
    public void Move(TobleGem gem)
    {
        if (moving != null) return;
        moving = gem;
        mouseStart = Input.mousePosition;
    }

    // Function that stop moving a TobleFood
    public void Drop()
    {
        if (moving == null) return;

        // If the TobleFood is at different and possible position, it swaps with this other TobleFood
        if(!(x_i == moving.x && y_i == moving.y) && outBoard(x_i, y_i))
        {
            BoardManager.instance.FlipGems(moving, BoardManager.instance.GetGem(x_i, y_i), true);
        }
        BoardManager.instance.ResetGem(moving);
        moving = null;
    }
}
