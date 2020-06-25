using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable Object that sets all the attributes and methods of a TobleFood
[CreateAssetMenu(fileName = "New TobleFood", menuName = "TobleFood ")]
public class TobleSO : ScriptableObject
{
    public Sprite artwork;
    public int points;
    public string tag;
    public AudioClip Select;

}
