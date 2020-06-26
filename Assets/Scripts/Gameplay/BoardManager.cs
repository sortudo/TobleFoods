using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to control and maintain the game board filled with organized toblefoods for the match 3 game
public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public List<TobleSO> TobleSOs = new List<TobleSO>();
    public GameObject TobleGem;
    public bool IsShifting { get; set; }
    public GameObject[,] tiles;

    public List<TobleGem> update;
    public List<FlippedGems> flipped;

    private TobleGem TobleScript;
    private RectTransform BoardM_r;
    private Vector2[] AdjacentDir = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        instance = GetComponent<BoardManager>();
        BoardM_r = GetComponent<RectTransform>();

        update = new List<TobleGem>();
        flipped = new List<FlippedGems>();

        CreateBoard();

        UIManager.instance.ScreenSizeChangeEvent += Align_BoardM;
        Align_BoardM();
    }

    void Update()
    {
        // Update TobleFoods if they need to keep or stop moving
        List<TobleGem> FinishedUpdating = new List<TobleGem>();
        for (int i = 0; i < update.Count; i++)
        {
            TobleGem gem = update[i];
            if (!gem.UpdateGem()) FinishedUpdating.Add(gem);
        }

        // After moving the TobleFoods, it needs to check if there is or is not a match
        // And what to do on each case
        for (int i = 0; i < FinishedUpdating.Count; i++)
        {
            TobleGem gem = FinishedUpdating[i];
            FlippedGems flip = getFlipped(gem);
            TobleGem flippedGem = null;

            List<TobleGem> connected = new List<TobleGem>();
            List<TobleGem> flip_connected = new List<TobleGem>();

            // If there was a swap in this update
            bool itFlipped = (flip != null);
            if (itFlipped) {
                // Get the connected matching list for the gem and flipped gem
                connected = GetMatching(gem);
                flippedGem = flip.OtherTobleGem(gem);
                flip_connected = GetMatching(flippedGem);

                // Add moved TobleFood if there is a match
                if (connected.Count > 1)
                    connected.Add(gem);
                if (flip_connected.Count > 1)
                    flip_connected.Add(flippedGem);
                connected.AddRange(flip_connected);
            }

            // If there is no matches
            if (connected.Count == 0)
            {
                // And TobleFoods were swapped
                if (itFlipped)
                    FlipGems(gem, flippedGem, false); // Swap them back
            }
            else // If there is matches
            {
                // Remove matching TobleFoods that are connected
                foreach (TobleGem tobleGem in connected){
                    if(tobleGem != null)
                    {
                        tobleGem.gameObject.SetActive(false);
                        // More things here in the next commit
                    }               
                }
            }
            flipped.Remove(flip);
            update.Remove(gem);
        }
    }

    // Function that search and returns matching TobleFoods, checking the horizontal and vertical Toblefoods
    public List<TobleGem> GetMatching(TobleGem Gem)
    {
        List<TobleGem> connected = new List<TobleGem>();

        connected.AddRange(GetMatching_Vertical(Gem));
        connected.AddRange(GetMatching_Horizontal(Gem));

        return connected;
    }

    // Function that check if there is any match at the vertical direction
    private List<TobleGem> GetMatching_Vertical(TobleGem Gem)
    {
        List<TobleGem> vertical_conn = new List<TobleGem>();

        // Above
        if (Gem.y - 1 >= 0 && tiles[Gem.x, Gem.y - 1].gameObject.tag == Gem.gameObject.tag)
        {
            vertical_conn.Add(GetGem(Gem.x, Gem.y - 1));
            // The top
            if (Gem.y - 2 >= 0 && tiles[Gem.x, Gem.y - 2].gameObject.tag == Gem.gameObject.tag)
                vertical_conn.Add(GetGem(Gem.x, Gem.y - 2));
        }
        // Below
        if (Gem.y + 1 < 8 && tiles[Gem.x, Gem.y + 1].gameObject.tag == Gem.gameObject.tag)
        {
            vertical_conn.Add(GetGem(Gem.x, Gem.y + 1));
            // The bottom
            if (Gem.y + 2 < 8 && tiles[Gem.x, Gem.y + 2].gameObject.tag == Gem.gameObject.tag)
                vertical_conn.Add(GetGem(Gem.x, Gem.y + 2));             
        }
        // To be a match it needs to be 2 or more
        if (vertical_conn.Count > 1)
            return vertical_conn;
        else return new List<TobleGem>();
    }

    // Function that check if there is any match at the horizontal direction
    private List<TobleGem> GetMatching_Horizontal(TobleGem Gem)
    {
        List<TobleGem> horizontal_conn = new List<TobleGem>();
        // Left
        if (Gem.x - 1 >= 0 && tiles[Gem.x - 1, Gem.y].gameObject.tag == Gem.gameObject.tag)
        {
            horizontal_conn.Add(GetGem(Gem.x - 1, Gem.y));
            // Far Left
            if (Gem.x - 2 >= 0 && tiles[Gem.x - 2, Gem.y].gameObject.tag == Gem.gameObject.tag)
                horizontal_conn.Add(GetGem(Gem.x - 2, Gem.y));    
        }
        // Right
        if (Gem.x + 1 < 8 && tiles[Gem.x + 1, Gem.y].gameObject.tag == Gem.gameObject.tag)
        {
            horizontal_conn.Add(GetGem(Gem.x + 1, Gem.y));
            // Far Right
            if (Gem.x + 2 < 8 && tiles[Gem.x + 2, Gem.y].gameObject.tag == Gem.gameObject.tag)
                horizontal_conn.Add(GetGem(Gem.x + 2, Gem.y));        
        }
        // To be a match it needs to be 2 or more
        if (horizontal_conn.Count > 1)
            return horizontal_conn;
        else return new List<TobleGem>();
    }

    // Function that align the BoardManager with the game board
    public void Align_BoardM()
    {
        BoardM_r.anchoredPosition = Board_side.instance.getPosition(-4, -3); 
    }

    // Function that reset the position of a TobleFood and update it
    public void ResetGem(TobleGem gem)
    {
        gem.ResetPosition();
        update.Add(gem);
    }

    // Function that search the flipped list and return the the first not null flipped's TobleGem
    public FlippedGems getFlipped(TobleGem Gem)
    {
        FlippedGems flipGem = null;
        for(int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].OtherTobleGem(Gem) != null){
                flipGem = flipped[i];
                break;
            }
        }

        return flipGem;
    }

    // Function that swap two TobleFoods
    public void FlipGems(TobleGem one, TobleGem two, bool flipflipped)
    {
        if (one == null) return;
        if (two != null)
        {
            // Swap their coordinates
            int aux_x = one.x;
            int aux_y = one.y;
            one.x = two.x;
            one.y = two.y;
            two.x = aux_x;
            two.y = aux_y;

            tiles[one.x, one.y] = one.gameObject;
            tiles[two.x, two.y] = two.gameObject;

            // Update their movement
            update.Add(one);
            update.Add(two);

            // If there is no matches and the swapped toblefoods need to swap back
            if (flipflipped)
                flipped.Add(new FlippedGems(one, two));
        }
        else
        {
            // Reset the position of a TobleFood and update it
            one.ResetPosition();
            update.Add(one);
        }
    }

    // Function that informs the TobleGem of a TobleFood at the board
    public TobleGem GetGem(int x, int y)
    {
        return tiles[x, y].GetComponent<TobleGem>();
    }

    // Function responsible to generate a correctly match 3 board
    private void CreateBoard()
    {
        tiles = new GameObject[8, 8];

        for(int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                // Create a new random TobleFood in the board
                GameObject newTile = Instantiate(TobleGem, new Vector3(0,0,0), TobleGem.transform.rotation);
                tiles[x, y] = newTile;

                // Search possible TobleFoods that can be used
                List<TobleSO> possibleSO = new List<TobleSO>();
                possibleSO.AddRange(TobleSOs);

                // Check above and left to avoid matches
                if(x-2 >= 0)
                {
                    if (tiles[x - 1, y].tag == tiles[x - 2, y].tag)
                        possibleSO.Remove(tiles[x - 1, y].GetComponent<TobleGem>().TobleSO);
                }
                if(y-2 >= 0) 
                {
                    if (tiles[x, y - 1].tag == tiles[x, y - 2].tag)
                        possibleSO.Remove(tiles[x, y - 1].GetComponent<TobleGem>().TobleSO);
                }

                TobleSO newSO = possibleSO[Random.Range(0, possibleSO.Count)];

                // Inform the characteristics of this TobleFood
                TobleScript = newTile.GetComponent<TobleGem>();
                TobleScript.TobleSO = newSO;
                TobleScript.x = x;
                TobleScript.y = y;

                newTile.transform.SetParent(transform, false); // BoardManager is the parent of all TobleFoods
            }
        }
    }
}

// Class to keep track of the swapped Gems 
[System.Serializable]
public class FlippedGems
{
    public TobleGem one;
    public TobleGem two;

    public FlippedGems(TobleGem o, TobleGem t)
    {
        one = o;
        two = t;
    }

    public TobleGem OtherTobleGem(TobleGem gem)
    {
        if (gem == one)
            return two;
        else if (gem == two)
            return one;
        else return null;
    }
}
