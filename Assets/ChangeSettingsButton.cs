using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class settingsButton : MonoBehaviour
{
    public int gameStartScene;

    // Called when we click the "settings" button.
    public void settingsButtonStart()
    {
        SceneManager.LoadScene(7);
    }
}
