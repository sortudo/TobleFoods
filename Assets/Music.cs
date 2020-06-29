using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible to not destroy Background music on load
public class Music : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
