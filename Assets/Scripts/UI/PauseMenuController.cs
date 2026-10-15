using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel de Pausa")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Nombre de la Escena del Menú Principal")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    public bool IsPaused => isPaused;

    private void Start()
    {
        // Al iniciar la escena, el menú de pausa debe estar oculto y el juego corriendo
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        ResumeGame();
    }

    private void Update()
    {
        // Al presionar Escape (ESC), alterna entre pausar y reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Pausa el tiempo del juego, activa el menú y desbloquea el cursor para interactuar con botones.
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f; // Congela la física, animaciones y movimiento

        // Muestra y libera el cursor del ratón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Reanuda el juego, oculta el menú y vuelve a bloquear el cursor para el modo FPS.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f; // Reanuda el tiempo normal

        // Oculta y bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Reinicia la escena actual desde cero.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Asegurar restaurar el tiempo antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Carga la escena del Menú Principal.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Asegurar restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
