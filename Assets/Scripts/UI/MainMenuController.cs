using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Escena del Juego")]
    [SerializeField] private string gameSceneName = "SampleScene";

    private void Start()
    {
        // En el menú principal, el tiempo debe correr normalmente y el cursor debe ser visible
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Inicia la partida cargando la escena de juego.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Cierra la aplicación (funciona en builds de juego).
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[MainMenu] Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        // Si estamos probando dentro del editor de Unity, detiene la simulación
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
