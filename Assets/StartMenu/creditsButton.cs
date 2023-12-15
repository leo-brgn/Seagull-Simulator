using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class creditsButton : MonoBehaviour
{
    public int gameStartScene;

    // Called when we click the "Play" button.
    public void creditsButtonStart()
    {
        SceneManager.LoadScene(3);
    }
}
