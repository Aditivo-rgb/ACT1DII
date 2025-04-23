using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool gameOver = false;

    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Si lo necesito en m�ltiples escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetGameOver()
    {
        gameOver = true;
        // Aqu� meto los men�s
        Debug.Log("�GAME OVER!");
        //Time.timeScale = 0f; // pausa el juego
        SceneManager.LoadScene("GameOver");
    }

    public void SetGameWin()
    {
        Debug.Log("He ganado");
        SceneManager.LoadScene("WinningScreen");
    }

   
        // Esto se llamar� cuando una nueva escena sea cargada
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
            if (scene.name == "GameOver" || scene.name == "WinningScreen" || scene.name == "TitleGame")
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


