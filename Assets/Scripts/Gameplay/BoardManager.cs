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
        // Update moving TobleFoods if there are still moving or not
        List<TobleGem> FinishedUpdating = new List<TobleGem>();
        for (int i = 0; i < update.Count; i++)
        {
            TobleGem gem = update[i];
            if (!gem.UpdateGem()) FinishedUpdating.Add(gem);
        }

        for (int i = 0; i < FinishedUpdating.Count; i++)
        {
            TobleGem gem = FinishedUpdating[i];
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

    // Function that swap TobleFoods
    public void FlipGems(TobleGem one, TobleGem two)
    {
        if (one == null) return;
        if (two != null)
        {
            TobleSO aux = one.TobleSO;
            one.TobleSO = two.TobleSO;
            two.TobleSO = aux;

            update.Add(one);
            update.Add(two);

            flipped.Add(new FlippedGems(one, two));

            one.SetGemCore();
            two.SetGemCore();
        }
        else
            ResetGem(one);
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

                // Check above and left
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
}
