using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] InputManagerSO inputManager;

    void OnEnable()
    {
        inputManager.OnPausar += TogglePause;
    }

    void OnDisable()
    {
        inputManager.OnPausar -= TogglePause;
    }

    void TogglePause()
    {
        if (gameIsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Rehabilitar input del juego
        inputManager.ActivarControles();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Deshabilitar input del juego
        inputManager.DesactivarControles();
    }

}
