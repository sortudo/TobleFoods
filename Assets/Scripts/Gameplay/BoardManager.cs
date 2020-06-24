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

    private GameObject[,] tiles;
    private TobleGem TobleScript;
    private RectTransform BoardM_r;

    void Start()
    {
        instance = GetComponent<BoardManager>();
        BoardM_r = GetComponent<RectTransform>();
        

        CreateBoard();
    }

    void Update()
    {
        BoardM_r.anchoredPosition = new Vector2(-Board_side.instance.size / 2, Board_side.instance.size * 3 / 8);  // Align the BoardManager with the game board
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
                newTile.GetComponent<Image>().sprite = newSO.artwork;
                newTile.tag = newSO.tag;

                newTile.transform.SetParent(transform, false); // BoardManager is the parent of all TobleFoods
            }
        }
    }
}
