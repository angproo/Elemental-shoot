using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int maxHealth = 100;

    [Header("Referencias")]
    [SerializeField] private GameOverController gameOverController;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverController == null)
        {
            gameOverController = FindFirstObjectByType<GameOverController>();
        }
    }

    /// <summary>
    /// Aplica daño a la salud del jugador.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"[Jugador Dañado] Vida actual: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Restaura la salud del jugador.
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log($"[Jugador Curado] Vida actual: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log("[Jugador Muerto] El jugador ha sido derrotado.");
        
        // Activa la pantalla de derrota si existe GameOverController en la escena
        if (gameOverController != null)
        {
            gameOverController.TriggerDefeat();
        }
    }
}
