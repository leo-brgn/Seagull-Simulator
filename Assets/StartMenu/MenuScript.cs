using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public int gameStartScene;

    // Called when we click the "Play" button.
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    // Called when we click the "Credits" button.
    public void OnCreditsButton()
    {
        SceneManager.LoadScene(3);
    }

    // Called when we click the "Exit" button.
    public void OnExitButton()
    {
        Application.Quit();
    }
}
