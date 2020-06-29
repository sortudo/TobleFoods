using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script to change scene
public class Change_scene : MonoBehaviour
{
    public string Scene;

    public void ChangetoScene()
    {
        SceneManager.LoadScene(sceneName: Scene);
    }
}
