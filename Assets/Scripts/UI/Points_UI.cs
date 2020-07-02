using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to create a Text showing the score that the player got from a match
public class Points_UI : MonoBehaviour
{
    public static Points_UI instance;
    public GameObject Points_txt;

    public void Start()
    {
        instance = GetComponent<Points_UI>();
    }

    public void Update()
    {
        transform.SetAsLastSibling(); // The text will be always in front of all TobleFoods
    }

    // Function that receive a position and points and place a Text there with the points' value
    public void CreatePointsUI(Vector2 pos, int points)
    {
        // Create new Points UI
        GameObject newPoint = Instantiate(Points_txt, new Vector3(0, 0, 0), Points_txt.transform.rotation);
        newPoint.transform.SetParent(this.gameObject.transform);
        RectTransform points_r = newPoint.GetComponent<RectTransform>();

        // Place it in the middle of the match
        points_r.anchoredPosition = pos;
        points_r.localScale = new Vector3(1,1,1);
        points_r.sizeDelta = new Vector2(Board_side.instance.size/4, Board_side.instance.size/12);
        
        newPoint.GetComponent<Text>().text = points.ToString();
        StartCoroutine(DestroyPointsUI());
    }

    // Function that after 2 seconds will destroy the primogenitor
    public IEnumerator DestroyPointsUI()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject.transform.GetChild(0).gameObject);
    }
}
