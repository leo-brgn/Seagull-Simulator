using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playButton : MonoBehaviour
{
    public int gameStartScene;

    // Called when we click the "Play" button.
    public void playButtonStart()
    {
        SceneManager.LoadScene(1);
    }
}
