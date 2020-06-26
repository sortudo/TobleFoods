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
    
    private TobleGem TobleScript;
    private RectTransform BoardM_r;

    public List<TobleGem> update;
    public List<FlippedGems> flipped;

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

            // Generate the connected matching list for gem
            List<TobleGem> connected = new List<TobleGem>(); // Raycast function will be aplied here

            // If there was a swap in this update
            bool itFlipped = (flip != null);
            if (itFlipped) {
                // Sum generate the connected matching list for the flipped gem with connected gem
                // Get the TobleGem of the swapped Gem
                flippedGem = flip.OtherTobleGem(gem);
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
                // Remove TobleFood that are connected
                foreach (TobleGem tobleGem in connected){
                    if(tobleGem != null)
                        tobleGem.gameObject.SetActive(false);
                    
                }
            }

            flipped.Remove(flip);
            update.Remove(gem);
        }
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

    // Function that returns a List of connected matching TobleFoods
    // Need to be done with raycast
    public List<TobleGem> ConnectedGems (int x, int y)
    {
        /*List<TobleGem> Connected = new List<TobleGem>();

        if(x + 1< 8)
            Connected.Add(GetGem(x + 1, y));
        else if (x - 1>= 0)
            Connected.Add(GetGem(x - 1, y));
        if(y + 1 < 8)
            Connected.Add(GetGem(x, y + 1));
        else if (y - 1>= 0)
            Connected.Add(GetGem(x, y - 1));
            */
        return new List<TobleGem>();
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
