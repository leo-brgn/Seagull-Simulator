using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScript : MonoBehaviour
{
    public GameObject gameMenuCanvas;
    public GameObject settingsCanvas;
    public GameObject seagull;

    private void Awake()
    {
        // Get canvas called "GameMenu"
        if (gameMenuCanvas == null)
        {
            gameMenuCanvas = GameObject.Find("GameMenu");
            if (gameMenuCanvas == null)
            {
                Debug.LogError("Could not find canvas called 'GameMenu'");
                return;
            }
        }

        // Get canvas called "Settings"
        if (settingsCanvas == null)
        {
            settingsCanvas = GameObject.Find("Settings");
            if (settingsCanvas == null)
            {
                Debug.LogError("Could not find canvas called 'Settings'");
                return;
            }
        }

        if (seagull == null)
        {
            seagull = GameObject.Find("Seagull");
            if (seagull == null)
            {
                Debug.LogError("Could not find seagull");
                return;
            }
        }
    }

    private void Start()
    {
        // Set both to inactive
        settingsCanvas.SetActive(false);
        gameMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!openMenu())
                closeMenu();
    }

    public bool openSettings()
    {
        if (!gameMenuCanvas.activeSelf)
        {
            Debug.LogError("Game menu canvas is not active");
            return false;
        }

        // Activate the settings canvas
        settingsCanvas.SetActive(true);
        // Deactivate the in-game menu canvas
        gameMenuCanvas.SetActive(false);
        return true;
    }

    public bool openMenu()
    {
        if (settingsCanvas.activeSelf) return false;
        if (gameMenuCanvas.activeSelf) return false;
        gameMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        Time.timeScale = 0;
        activateMouseCursor();
        return true;
    }

    public bool returnToMenu()
    {
        if (!settingsCanvas.activeSelf)
        {
            Debug.LogError("Settings canvas is not active");
            return false;
        }

        // Deactivate the settings canvas
        settingsCanvas.SetActive(false);
        // Activate the in-game menu canvas
        gameMenuCanvas.SetActive(true);
        return true;
    }

    public bool closeMenu()
    {
        if (!gameMenuCanvas.activeSelf)
        {
            Debug.LogError("Game menu canvas is not active");
            return false;
        }

        gameMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        Time.timeScale = 1;
        deactivateMouseCursor();
        return true;
    }

    public void deactivateMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void activateMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}