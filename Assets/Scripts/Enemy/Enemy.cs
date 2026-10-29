using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuración Elemental")]
    [SerializeField] private ElementType enemyElement = ElementType.Fire; // Elemento al que pertenece el enemigo

    [Header("Vida")]
    [SerializeField] private int health = 3;

    [Header("Movimiento y Ataque")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float detectionRange = 15.0f;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private Transform playerTransform;
    private float nextAttackTime = 0f;

    // Propiedades públicas para consultar datos desde otros scripts
    public ElementType EnemyElement => enemyElement;
    public int Health => health;

    private void Start()
    {
        // Busca automáticamente al jugador por su Tag "Player" en la escena
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Si el jugador está en rango de detección pero no en rango de ataque -> Avanza hacia el jugador
        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        // Si está en rango de ataque -> Intenta atacar
        else if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calcula la dirección hacia el jugador manteniendo la altura (eje Y)
        Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Orienta al enemigo mirando al jugador
        transform.LookAt(targetPosition);
    }

    private void AttackPlayer()
    {
        nextAttackTime = Time.time + attackCooldown;

        // Intenta obtener el componente de vida del jugador (PlayerHealth)
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        Debug.Log($"[Ataque] ¡{gameObject.name} ({enemyElement}) atacó al jugador causando {attackDamage} de daño!");
    }

    /// <summary>
    /// Recibe daño si la bala utilizada corresponde al elemento super-efectivo contra este enemigo.
    /// Relación Circular: Fire > Ice > Electricity > Acid > Fire
    /// </summary>
    /// <returns>True si el impacto fue efectivo, false si fue inmune.</returns>
    public bool TakeDamage(ElementType bulletElement)
    {
        ElementType effectiveElement = GetEffectiveElementFor(enemyElement);

        if (bulletElement == effectiveElement)
        {
            health--;
            Debug.Log($"[Daño Efectivo] ¡{bulletElement} dañó a {gameObject.name} ({enemyElement})! Vida restante: {health}");

            if (health <= 0)
            {
                Die();
            }
            return true;
        }
        else
        {
            Debug.Log($"[Inmune] {gameObject.name} ({enemyElement}) es inmune a balas de {bulletElement}. Usa {effectiveElement}.");
            return false;
        }
    }

    /// <summary>
    /// Retorna el elemento que es SUPER EFECTIVO contra el elemento del enemigo.
    /// Regla:
    /// - Fuego (Fire) vence a Hielo (Ice)          -> Balas de FUEGO dañan a enemigo de HIELO.
    /// - Hielo (Ice) vence a Electricidad           -> Balas de HIELO dañan a enemigo de ELECTRICIDAD.
    /// - Electricidad vence a Ácido (Acid)          -> Balas de ELECTRICIDAD dañan a enemigo de ÁCIDO.
    /// - Ácido (Acid) vence a Fuego (Fire)          -> Balas de ÁCIDO dañan a enemigo de FUEGO.
    /// </summary>
    private ElementType GetEffectiveElementFor(ElementType targetEnemyElement)
    {
        switch (targetEnemyElement)
        {
            case ElementType.Ice:
                return ElementType.Fire;        // Fire > Ice
            case ElementType.Electricity:
                return ElementType.Ice;         // Ice > Electricity
            case ElementType.Acid:
                return ElementType.Electricity; // Electricity > Acid
            case ElementType.Fire:
                return ElementType.Acid;        // Acid > Fire
            default:
                return ElementType.Fire;
        }
    }

    private void Die()
    {
        Debug.Log($"[Muerte] Enemigo {gameObject.name} ({enemyElement}) ha sido derrotado.");
        Destroy(gameObject);
    }
}