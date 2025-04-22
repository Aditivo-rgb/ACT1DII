using UnityEngine;

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
    }

    public void SetGameWin()
    {
        Debug.Log("He ganado");
    }
}
