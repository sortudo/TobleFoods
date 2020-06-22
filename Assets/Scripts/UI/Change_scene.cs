using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_scene : MonoBehaviour
{
    public Object Scene;

    public void ChangetoScene()
    {
        SceneManager.LoadScene(sceneName: Scene.name);
    }
}
