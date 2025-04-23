using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    public void ExitScene()
    {
        Time.timeScale = 1f; // asegurarse de que el tiempo no sigue en 0
        SceneManager.LoadScene("TitleGame");
        Debug.Log("Saliendo del juego...");
    }

    public void PlayGame()
    {
        Time.timeScale = 1f; // asegurarse de que el tiempo no sigue en 0
        SceneManager.LoadScene("Escena_Noche");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        // En el editor de Unity, esto no cierra el juego, así que ponemos un mensaje.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
