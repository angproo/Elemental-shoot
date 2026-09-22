using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
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
        // En la Fase 5 se activará la pantalla de GameOver
    }
}
