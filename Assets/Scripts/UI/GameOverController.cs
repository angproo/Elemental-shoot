using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("Panel de Fin de Juego")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Texto de Título / Resultado")]
    [SerializeField] private TextMeshProUGUI resultTitleText;

    [Header("Mensajes y Colores")]
    [SerializeField] private string victoryMessage = "¡VICTORIA!";
    [SerializeField] private Color victoryColor = new Color(0.2f, 1f, 0.3f); // Verde brillante
    [SerializeField] private string defeatMessage = "DERROTA";
    [SerializeField] private Color defeatColor = new Color(1f, 0.2f, 0.2f);   // Rojo

    [Header("Nombre de la Escena del Menú Principal")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    private void Start()
    {
        // El panel de fin de partida inicia desactivado
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Activa la pantalla de fin de juego configurando el título y colores correspondientes.
    /// </summary>
    /// <param name="isVictory">true para victoria, false para derrota</param>
    public void TriggerGameOver(bool isVictory)
    {
        if (isGameOver) return; // Evita llamadas duplicadas
        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            if (isVictory)
            {
                resultTitleText.text = victoryMessage;
                resultTitleText.color = victoryColor;
            }
            else
            {
                resultTitleText.text = defeatMessage;
                resultTitleText.color = defeatColor;
            }
        }

        // Congela el juego y libera el cursor para poder pulsar los botones
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Helper para activar Victoria.
    /// </summary>
    public void TriggerVictory()
    {
        TriggerGameOver(true);
    }

    /// <summary>
    /// Helper para activar Derrota.
    /// </summary>
    public void TriggerDefeat()
    {
        TriggerGameOver(false);
    }

    /// <summary>
    /// Reinicia la escena actual desde el principio.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaurar el tiempo normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Vuelve a la escena del Menú Principal.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Restaurar el tiempo normal
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
