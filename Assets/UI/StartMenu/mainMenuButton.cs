using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuButton : MonoBehaviour
{
    // Called when we click the "Play" button.
    public void mainMenuButtonStart()
    {
        SceneManager.LoadScene(2);
    }
}
