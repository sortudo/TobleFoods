using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to control and maintain the game board filled with organized toblefoods for the match 3 game
public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public GameObject TobleGem; // Toble Prefab
    public GameObject[,] tiles; // Board
    // 0 - Left | 1 - Right | 2 - Up | 3 - Down
    // 4 - Up Left | 5 - Up Right | 6 - Down Left | 7 -Down Right
    private Vector2[] direction = { new Vector2(-1, 0),new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1),
                                    new Vector2(-1,-1), new Vector2(1,-1), new Vector2(-1, 1), new Vector2(1,1)};

    // Lists
    public List<TobleGem> update; // Moving TobleFoods
    public List<TobleGem> FinishedUpdating; // TobleFoods in this frame that stopped moving
    public List<FlippedGems> flipped; // Swapped TobleFoods
    public List<TobleGem> destroyed; // Destroyed TobleFoods
    public List<TobleSO> TobleSOs = new List<TobleSO>(); // TobleSOs

    // SFX
    public AudioClip Clear;
    public AudioClip Swap;
    public AudioClip Shuffle;
    public AudioClip Shuffle_voice;

    public int[] fills; // The number of empty spaces in which column
    public int points_move; // Points in a single swap
    public float combo; // Number of of matches in a single swap that will multiply the score
    public Possible_Match pm; // Hint for the player
    public Vector2 max = new Vector2(-Mathf.Infinity, -Mathf.Infinity); // Max position of the match
    public Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity); // Min position of the match
    public bool reseting; // Check if the board if reseting

    // Private
    private TobleGem TobleScript;
    private RectTransform BoardM_r;

    void Start()
    {

        instance = GetComponent<BoardManager>();
        BoardM_r = GetComponent<RectTransform>();

        update = new List<TobleGem>();
        flipped = new List<FlippedGems>();
        destroyed = new List<TobleGem>();
        fills = new int[8];
        points_move = 0;
        combo = 1.0f;
        pm = null;
        reseting = false;

        CreateBoard();

        UIManager.instance.ScreenSizeChangeEvent += Align_BoardM;
        Align_BoardM();
    }

    void Update()
    {
        // Update TobleFoods if they need to keep or stop moving
        FinishedUpdating = new List<TobleGem>();
        for (int i = 0; i < update.Count; i++)
        {
            TobleGem gem = update[i];
            if (gem != null && !gem.UpdateGem()) FinishedUpdating.Add(gem);
        }

        // After moving the all the TobleFoods, it needs to check if there is or is not a match
        // And what to do on each case
        for (int i = 0; i < FinishedUpdating.Count; i++)
        {
            TobleGem gem = FinishedUpdating[i];
            FlippedGems flip = getFlipped(gem);
            TobleGem flippedGem = null;

            // Board's empty spaces that will need to fill in
            int x = gem.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, 8);

            // Get the connected matching list for the gem
            List<TobleGem> connected = GetMatching(gem);
            List<TobleGem> flip_connected = new List<TobleGem>();
            // If it founds a match, adds the moving TobleFood
            if (connected.Count > 1)
                connected.Add(gem);

            // If there was a swap in this update
            bool itFlipped = (flip != null);
            if (itFlipped) {
                // Gets the connected matching list for the flipped gem
                flippedGem = flip.OtherTobleGem(gem);
                flip_connected = GetMatching(flippedGem);

                // Adds moved TobleFood if there is a match
                if (flip_connected.Count > 1)
                    flip_connected.Add(flippedGem);
                connected.AddRange(flip_connected);
            }

            // If there is no matches
            if (connected.Count == 0)
            {
                // And TobleFoods were swapped
                if (itFlipped)
                {
                    FlipGems(gem, flippedGem, false, false); // Swap them back

                    // Reset hint
                    StopAllCoroutines();
                    if (pm != null)
                    {
                        pm.DyingLight();
                        StartCoroutine(pm.Coroutine_HighlightIt());
                    }
                }
                    
            }
            else if(connected.Count > 2) // If there is matches
            {
                SFXManager.instance.PlaySFX(Clear);

                // Reset Hint
                if(pm != null)
                    pm.NoHighlight();

                // Destroy matching TobleFoods that are connected
                foreach (TobleGem tobleGem in connected){
                    if(tobleGem != null && !destroyed.Contains(tobleGem))
                    {
                        tobleGem.gameObject.tag = "TobleDestroyed";
                        tobleGem.Gem_a.SetBool("DestroyIt", true);
                        tobleGem.transform.SetAsLastSibling();
                        tobleGem.Gem_p.gameObject.SetActive(true);

                        // Points will be increased each round
                        int economy = (tobleGem.TobleSO.points + ((StatusManager.instance.next_round - 1) * tobleGem.TobleSO.points));

                        // If 3 or more toblefoods have already been destroyed, triple the points earned for each next destroyed
                        if (destroyed.Count > 2)
                            points_move += (3 * economy) * (int)combo;
                        else
                            points_move += economy * (int)combo;

                        combo += (float)1/connected.Count; // Multiply points for each match done in one single swap

                        destroyed.Add(tobleGem);
                    }               
                }
                combo = Mathf.Round(combo);
            }
            flipped.Remove(flip);
            update.Remove(gem);
        }
    }

    // Function that destroy a TobleFood after a match
    public void DestroyGemOnBoard()
    {
        // Calculate the points UI position that will pop-up showing the match score
        // Find the min and max positions 
        if (destroyed[0].Gem_r.anchoredPosition.x < min.x)
            min.x = destroyed[0].Gem_r.anchoredPosition.x;
        else if (destroyed[0].Gem_r.anchoredPosition.x > max.x)
            max.x = destroyed[0].Gem_r.anchoredPosition.x;

        if (destroyed[0].Gem_r.anchoredPosition.y < min.y)
            min.y = destroyed[0].Gem_r.anchoredPosition.y;
        else if (destroyed[0].Gem_r.anchoredPosition.y > max.y)
            max.y = destroyed[0].Gem_r.anchoredPosition.y;

        // Check if all TobleFoods from the matching list were destroyed correctly
        // If so, Update the Board and Points
        destroyed.RemoveAt(0);
        if (destroyed.Count == 0)
        {
            // Play Narrator voice for each combo
            if (combo == 3)
                Narrator.instance.PlayCombo(0);
            else if (combo == 4)
                Narrator.instance.PlayCombo(1);
            else if (combo == 5)
                Narrator.instance.PlayCombo(2);
            else if (combo > 5)
                Narrator.instance.PlayCombo(3);

            ApplyGravityToTobleFood();
            StatusManager.instance.UpdateScore(points_move);

            // Points UI will pop-up between max and min
            Points_UI.instance.CreatePointsUI((max + min) / 2, points_move);
            points_move = 0;

            // Reset variables for the next movement
            max = new Vector2(-Mathf.Infinity, -Mathf.Infinity);
            min = new Vector2(Mathf.Infinity, Mathf.Infinity);
            destroyed = new List<TobleGem>();

            Verify_Board();
        }
    }

    // Function that checks if this board has any possible moves
    public void Verify_Board()
    {
        List<Possible_Match> p = Possible_Matches();
        if (p.Count > 0) // If it does
        {
            // Highlight a possible match
            if (pm != null)
            {
                StopAllCoroutines();
                pm.NoHighlight();
            }
            StartCoroutine(p[Random.Range(0, p.Count)].Coroutine_HighlightIt());
        }
        else
        {
            // If it does not, destroy the current board and create a new one
            StartCoroutine(ResetBoard());
        }
    }

    //  Function that destroys and creates a brand new board
    public IEnumerator ResetBoard()
    {
        reseting = true;
        pm = null;
        Narrator.instance.PlayNarrator(Shuffle_voice, false);
        yield return new WaitForSeconds(1);
        StatusSound.instance.PlayStatus(Shuffle);
        DestroyBoard();
        yield return new WaitForSeconds(1);
        CreateBoard();
        reseting = false;
    }

    // Function that removes from play every TobleFood on the board
    public void DestroyBoard()
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                GetGem(x, y).Gem_a.SetBool("Disappear", true);
            }
    }

    // Function that applies gravity so that Toblefoods can fall if there is an empty space
    public void ApplyGravityToTobleFood()
    {
        // Search for empty spaces
        for(int x = 0; x < 8; x++)
            for(int y = 7; y >= 0; y--)
            {
                // Search for Destroyed TobleFoods on the game board
                // If the search finds it, check above to fill the empty space
                if (tiles[x, y].tag != "TobleDestroyed") continue;
                for(int ny = (y-1); ny >= -1; ny--)
                {
                    if(ny >= 0) // Searching in the grid
                    {
                        // If it is not a Destroyed TobleFood, make it fall
                        if (tiles[x, ny].tag == "TobleDestroyed") continue;
                        FlipGems(GetGem(x, y), GetGem(x, ny), false, true);
                    }
                    else // Hit the end of the game board
                    {
                        // Create new random TobleFood to fill the destroyed TobleFood
                        TobleSO newSO = TobleSOs[Random.Range(0, TobleSOs.Count)];

                        // Inform the characteristics of this TobleFood
                        TobleScript = tiles[x, y].GetComponent<TobleGem>();
                        TobleScript.TobleSO = newSO;
                        TobleScript.UpdateInterface();

                        // Change its position to fall at the top of the board
                        TobleScript.Gem_r.anchoredPosition = Board_side.instance.getPosition(x, -1 - fills[x]) + new Vector2(Board_side.instance.size / 16, Board_side.instance.size / 16);
                        ResetGem(TobleScript);
                        fills[x]++;
                    } 
                    break;
                }
            }
    }

    // Function that returns the list of possible matches of a TobleFood
    private List<Possible_Match> Find_PossibleAtThis(TobleGem gem)
    {
        List<Possible_Match> p_poss = new List<Possible_Match>();
        // Foreach neighbour
        foreach (Vector2 dir in direction)
        {
            // Check if this neighbour is at range and if it has the same tag
            Vector2 check = new Vector2(gem.x, gem.y) + dir;
            if (CheckDir(check, gem))
            {
                TobleGem c = GetGem((int)check.x, (int)check.y);
                Possible_Match p;
                Possible_Match p_vh = new Possible_Match(gem, c);
                // If this direction is Vertical or Horizontal
                if (dir.x == 0 || dir.y == 0)
                {
                    // Check for i shape
                    Vector2 checkUp = new Vector2(c.x, c.y) + dir * 2;    //| g | c | - | p |
                    p = Add_Poss(checkUp, c, p_vh);
                    if(p != null)
                        p_poss.Add(p);
                    // Check for L shape
                    if(dir.x == 0)
                    {                           
                        Vector2 checkRight = new Vector2(c.x, c.y) + dir + new Vector2(1, 0);  //     | - | p |
                        p = Add_Poss(checkRight, c, p_vh);                                     //     | c |    
                        if (p != null)                                                         //     | g |
                            p_poss.Add(p);

                        Vector2 checkLeft = new Vector2(c.x, c.y) + dir + new Vector2(-1, 0);  // | p | - | 
                        p = Add_Poss(checkLeft, c, p_vh);                                      //     | c |
                        if (p != null)                                                         //     | g | 
                            p_poss.Add(p);
                    }
                    else
                    {
                        Vector2 checkRight = new Vector2(c.x, c.y) + dir + new Vector2(0, 1);  // | g | c | - |
                        p = Add_Poss(checkRight, c, p_vh);                                     //         | p |
                        if (p != null)
                            p_poss.Add(p);

                        Vector2 checkLeft = new Vector2(c.x, c.y) + dir + new Vector2(0,-1);   //         | p |
                        p = Add_Poss(checkLeft, c, p_vh);                                      // | g | c | - |
                        if (p != null)
                            p_poss.Add(p);
                    }
                }
                else // If this direction is Diagonal - Search for V shape
                {
                    if(dir.x < 0) // Left
                    {
                        Vector2 checkLeft = new Vector2(gem.x, gem.y) + new Vector2(-2, 0);   //     | c |
                        p = Add_Poss(checkLeft, gem, p_vh);                                   // | p | - | g |
                        if (p != null)                                                        //     | c |
                            p_poss.Add(p);
                    }
                    else // Right
                    {
                        Vector2 checkRight = new Vector2(gem.x, gem.y) + new Vector2(2, 0);   //     | c |
                        p = Add_Poss(checkRight, gem, p_vh);                                  // | g | - | p |
                        if (p != null)                                                        //     | c |
                            p_poss.Add(p);
                    }

                    if(dir.y < 0) // Up
                    {
                        Vector2 checkUp = new Vector2(gem.x, gem.y) + new Vector2(0,-2);   //     | p |
                        p = Add_Poss(checkUp, gem, p_vh);                                  // | c | - | c |
                        if (p != null)                                                     //     | g |      
                            p_poss.Add(p);
                    }
                    else // Down
                    {
                        Vector2 checkDown = new Vector2(gem.x, gem.y) + new Vector2(0, 2);   //     | g |
                        p = Add_Poss(checkDown, gem, p_vh);                                  // | c | - | c |
                        if (p != null)                                                       //     | p |       
                            p_poss.Add(p);
                    }
                }
            }
        }
        return p_poss;
    }

    // Function that returns a possible match on this direction
    private Possible_Match Add_Poss(Vector2 check, TobleGem gem, Possible_Match p)
    {
        if (CheckDir(check, gem))
        {
            p.third = GetGem((int)check.x, (int)check.y);
            return p;
        }
        return null; 
    }

    // Function that returns true if on this direction has a TobleFood with the same tag
    private bool CheckDir(Vector2 check, TobleGem gem)
    {
        return (check.x >= 0 && check.x < 8 && check.y >= 0 && check.y < 8 && tiles[(int)check.x, (int)check.y].gameObject.tag == gem.gameObject.tag);
    }

    // Function that for each TobleFood on the board find its possible matches
    public List<Possible_Match> Possible_Matches()
    {
        List<Possible_Match> possible = new List<Possible_Match>();
        for (int y = 0; y < 8; y++)
            for(int x = 0; x < 8; x++)
            {
                List <Possible_Match> atThisP = Find_PossibleAtThis(GetGem(x,y));
                if (atThisP.Count > 0)
                    possible.AddRange(atThisP);
            }

        return possible;
    } 

    // Function that search and returns matching TobleFoods, checking the horizontal and vertical Toblefoods
    public List<TobleGem> GetMatching(TobleGem Gem)
    {
        List<TobleGem> connected = new List<TobleGem>();

        connected.AddRange(GetMatching_Vertical(Gem));
        connected.AddRange(GetMatching_Horizontal(Gem));

        return connected;
    }

    // Function that return matches at the vertical direction
    private List<TobleGem> GetMatching_Vertical(TobleGem Gem)
    {
        List<TobleGem> vertical_conn = new List<TobleGem>();

        // Above
        vertical_conn.AddRange(GetMatching_Above(Gem));
        // Below
        vertical_conn.AddRange(GetMatching_Below(Gem));

        // To be a match it needs to be 2 or more
        if (vertical_conn.Count > 1)
            return vertical_conn;
        else return new List<TobleGem>();
    }

    // Function thatreturn matches above
    private List<TobleGem> GetMatching_Above(TobleGem Gem)
    {
        List<TobleGem> above_conn = new List<TobleGem>();

        if (Gem.y - 1 >= 0 && tiles[Gem.x, Gem.y - 1].gameObject.tag == Gem.gameObject.tag && !GetGem(Gem.x, Gem.y - 1).updating)
        {
            TobleGem newGem = GetGem(Gem.x, Gem.y - 1);
            above_conn.Add(newGem);
            above_conn.AddRange(GetMatching_Above(newGem));
        }
        return above_conn;
    }

    // Function that return matches below
    private List<TobleGem> GetMatching_Below(TobleGem Gem)
    {
        List<TobleGem> below_conn = new List<TobleGem>();

        if (Gem.y + 1 < 8 && tiles[Gem.x, Gem.y + 1].gameObject.tag == Gem.gameObject.tag && !GetGem(Gem.x, Gem.y + 1).updating)
        {
            TobleGem newGem = GetGem(Gem.x, Gem.y + 1);
            below_conn.Add(newGem);
            below_conn.AddRange(GetMatching_Below(newGem));
        }
        return below_conn;
    }

    // Function that return matches at the horizontal direction
    private List<TobleGem> GetMatching_Horizontal(TobleGem Gem)
    {
        List<TobleGem> horizontal_conn = new List<TobleGem>();
        // Left
        horizontal_conn.AddRange(GetMatching_Left(Gem));
        // Right
        horizontal_conn.AddRange(GetMatching_Right(Gem));

        // To be a match it needs to be 2 or more
        if (horizontal_conn.Count > 1)
            return horizontal_conn;
        else return new List<TobleGem>();
    }

    // Function that return matches at the left
    private List<TobleGem> GetMatching_Left(TobleGem Gem)
    {
        List<TobleGem> left_conn = new List<TobleGem>();

        if (Gem.x - 1 >= 0 && tiles[Gem.x - 1, Gem.y].gameObject.tag == Gem.gameObject.tag && !GetGem(Gem.x - 1, Gem.y).updating)
        {
            TobleGem newGem = GetGem(Gem.x - 1, Gem.y);
            left_conn.Add(newGem);
            left_conn.AddRange(GetMatching_Left(newGem));
        }
        return left_conn;
    }

    // Function that return matches at the right
    private List<TobleGem> GetMatching_Right(TobleGem Gem)
    {
        List<TobleGem> right_conn = new List<TobleGem>();

        if (Gem.x + 1 < 8 && tiles[Gem.x + 1, Gem.y].gameObject.tag == Gem.gameObject.tag && !GetGem(Gem.x + 1, Gem.y).updating)
        {
            TobleGem newGem = GetGem(Gem.x + 1, Gem.y);
            right_conn.Add(newGem);
            right_conn.AddRange(GetMatching_Right(newGem));
        }
        return right_conn;
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
    public void FlipGems(TobleGem one, TobleGem two, bool flipflipped, bool gravity)
    {
        if (one == null) return;
        if (two != null)
        {

            if (!gravity)
                SFXManager.instance.PlaySFX(Swap);
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

    // Function that informs the TobleGem of a TobleFood on the board
    public TobleGem GetGem(int x, int y)
    {
        return tiles[x, y].GetComponent<TobleGem>();
    }

    // Function responsible to generate a correctly match 3 board
    public void CreateBoard()
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
                newTile.gameObject.tag = newSO.tag;

                newTile.transform.SetParent(transform, false); // BoardManager is the parent of all TobleFoods
            }
        }
        Verify_Board();
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

// Class to keep track of the possible matches on the board
public class Possible_Match
{
    public TobleGem first;
    public TobleGem second;
    public TobleGem third;

    public Possible_Match(TobleGem f, TobleGem s)
    {
        first = f;
        second = s;
        third = null;
    }

    // Function that activates the hint for the player
    public void HighlightIt()
    {
        first.Gem_a.SetBool("Highlight", true);
        second.Gem_a.SetBool("Highlight", true);
        third.Gem_a.SetBool("Highlight", true);

        BoardManager.instance.pm = this;
    }

    // Function that waits 10 seconds to show the hint
    public IEnumerator Coroutine_HighlightIt()
    {
        BoardManager.instance.pm = this;
        yield return new WaitForSeconds(10);
        if(BoardManager.instance.pm == this)
            HighlightIt();
    }

    // Function that deactivates the current hint
    public void NoHighlight()
    {
        if (BoardManager.instance.pm != null)
        {
            first.Gem_a.SetBool("Highlight", false);
            second.Gem_a.SetBool("Highlight", false);
            third.Gem_a.SetBool("Highlight", false);

            BoardManager.instance.pm = null;
        }
    }

    // Function that just turn off the Highlight 
    public void DyingLight()
    {
        first.Gem_a.SetBool("Highlight", false);
        second.Gem_a.SetBool("Highlight", false);
        third.Gem_a.SetBool("Highlight", false);
    }
}
