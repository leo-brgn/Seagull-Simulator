using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class resumeButton : MonoBehaviour
{
    private string gameScene;  // Almacena el nombre de la escena del juego

    // Variables que deseas persistir entre escenas o men�s
    private int nivelDeVida = 100;
    private int cantidadDeObjetos = 0;

    private void Start()
    {
        // Obtener el nombre de la escena actual
        gameScene = SceneManager.GetActiveScene().name;

        // Asignar la funci�n ManejarBotonSalirConfiguracion() al evento de clic del bot�n
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(resume);
        }
    }

    private void resume()
    {
        // Guardar el estado actual de las variables antes de cambiar de escena
        saveVariables();

        // Cargar la escena del juego (puedes personalizar esta l�gica seg�n tus necesidades)
        SceneManager.LoadScene(gameScene);

        Debug.Log("Exiting settings menu");
    }

    private void saveVariables()
    {
        // Guardar el estado actual de las variables que deseas persistir
        PlayerPrefs.SetInt("NivelDeVida", nivelDeVida);
        PlayerPrefs.SetInt("CantidadDeObjetos", cantidadDeObjetos);

        // Puedes guardar m�s variables seg�n tus necesidades
        PlayerPrefs.Save();
    }
}

