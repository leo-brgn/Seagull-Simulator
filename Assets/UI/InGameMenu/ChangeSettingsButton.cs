using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class settingsButton : MonoBehaviour
{
    public int gameStartScene;

    public GUIScript guiScript;
    
    // Called when we click the "settings" button.
    public void settingsButtonStart()
    {
        guiScript.openSettings();
    }
}
