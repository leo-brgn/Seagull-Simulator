using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class resumeButton : MonoBehaviour
{
    public GUIScript guiScript;
    
    // Called when we click the "continue" button.
    public void resume()
    {
        guiScript.closeMenu();
    }
}

