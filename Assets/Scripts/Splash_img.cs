using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Splash_img : MonoBehaviour
{

    const float ImageWidth = 1000.0f,
                TimeOut = 8.0f;

    public enum SplashStates
    {
        Moving,   //The splash image is moving on the screen
        Finish
    }  //Time out, a pressed key or the splash image is just moved, go to main menu scene

    public SplashStates State;
    public Vector3 Speed = new Vector3(-60.0f, -60.0f, 0.0f);  //Speed for moving the image on the screen
    
    float startTime;

    Image image;
    Color32 c;

    // Use this for initialization
    void Start()
    {
        State = SplashStates.Moving;
        startTime = Time.time;
        image = GetComponent<Image>();
        c = new Color32(0, 0, 0, 255);
        image.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case SplashStates.Moving:   //The splash image is moving on the screen
                transform.Translate (Speed * Time.deltaTime);
                if (c.r < 255) c.r += 1;
                if (c.g < 255) c.g += 1;
                if (c.b < 255) c.b += 1;
                image.color = c;

                if (Time.time - startTime > TimeOut - 1.0f)    //También se puede acabar la presentación por tiempo
                    Speed = new Vector3(0.0f, 0.0f, 0.0f);

                if (Time.time - startTime > TimeOut)    //También se puede acabar la presentación por tiempo
                    State = SplashStates.Finish;

                if (Input.GetKey(KeyCode.Escape) || //Si se pulsa la tecla escape
                    Input.GetKey(KeyCode.Return) || //Si se pulsa la tecla enter
                    Input.GetKey(KeyCode.Space))    //Si se pulsa la tecla espacio

                    State = SplashStates.Finish;
                break;
            case SplashStates.Finish:
                SceneManager.LoadScene("MenuPpal");
                break;
        }

    }
}
