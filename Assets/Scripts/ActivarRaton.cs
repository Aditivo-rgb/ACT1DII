using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Esto se llamará cuando una nueva escena sea cargada
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si estamos en una escena de Game Over o Victoria, activamos el cursor
        if (scene.name == "GameOver" || scene.name == "WinningScreen") 
        {
            Debug.Log("cambio de escena");
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;  // Desbloquear el cursor
        Cursor.visible = true;                   // Hacer visible el cursor
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor al centro
        Cursor.visible = false;                   // Hacer invisible el cursor
    }
}
