using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backButton : MonoBehaviour
{
    // Called when we click the "Play" button.
    public void backButtonStart()
    {
        SceneManager.LoadScene(2);
    }
}
