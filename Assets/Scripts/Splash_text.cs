using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI; // This is so that it should find the Text component

public class Splash_text : MonoBehaviour
{
    Color oc;   //Original Color

    TMPro.TextMeshProUGUI texto;

    [SerializeField]
    float angulo, velAng = 1.0f;

    void Start()
    {
        texto = GetComponent<TMPro.TextMeshProUGUI>();
        oc = texto.color;
        if (velAng < 0.0f) velAng = -velAng;
    }

    void Update()
    {
        float seno = Mathf.Abs(Mathf.Sin(angulo));

        angulo += velAng * Time.deltaTime;
        if (angulo > 360.0f) angulo -= 360.0f;

        texto.color = oc * seno;
    }

}
