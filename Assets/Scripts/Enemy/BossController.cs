using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int maxHealth = 100;

    [Header("Movimiento y Ataque")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float detectionRange = 25.0f;
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 2.0f;

    [Header("Disparo a Distancia")]
    [SerializeField] private bool canShootProjectiles = true;
    [SerializeField] private float shootCooldown = 2.5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ProjectileFactory projectileFactory;
    [SerializeField] private AudioClip shootSound;

    [Header("Indicador Visual y Materiales por Fase")]
    [SerializeField] private Renderer bossRenderer;
    [SerializeField] private Light bossLight;
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material iceMaterial;
    [SerializeField] private Material electricityMaterial;
    [SerializeField] private Material acidMaterial;

    [Header("Colores por Fase")]
    [SerializeField] private Color fireColor = new Color(1f, 0.3f, 0.1f);
    [SerializeField] private Color iceColor = new Color(0.2f, 0.7f, 1f);
    [SerializeField] private Color electricityColor = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private Color acidColor = new Color(0.3f, 1f, 0.2f);

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource bossAudioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip phaseChangeSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0f, 1f)] [SerializeField] private float bossVolume = 1f;

    // Estado interno
    private int currentHealth;
    private ElementType currentPhase = ElementType.Fire;
    private int currentPhaseIndex = 0; // 0: Fire, 1: Ice, 2: Electricity, 3: Acid
    private bool isDead = false;
    private float nextAttackTime = 0f;
    private float nextShootTime = 0f;
    private bool hasTriggeredBossMusic = false;
    private Transform playerTransform;
    private GameOverController gameOverController;

    // Eventos para desacoplar audio, HUD y Game Loop
    public event Action<ElementType> OnPhaseChanged;
    public event Action<int, int> OnHealthChanged; // currentHealth, maxHealth
    public event Action OnBossDeath;

    // Propiedades públicas
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public ElementType CurrentPhase => currentPhase;
    public bool IsDead => isDead;

    private void Awake()
    {
        if (bossRenderer == null)
        {
            bossRenderer = GetComponentInChildren<Renderer>();
        }

        if (projectileFactory == null)
        {
            projectileFactory = FindFirstObjectByType<ProjectileFactory>();
        }

        if (bossLight == null)
        {
            bossLight = GetComponentInChildren<Light>();
        }

        if (bossAudioSource == null)
        {
            bossAudioSource = GetComponent<AudioSource>();
            if (bossAudioSource == null)
            {
                bossAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentPhase = ElementType.Fire;
        currentPhaseIndex = 0;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        if (gameOverController == null)
        {
            gameOverController = FindFirstObjectByType<GameOverController>();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        UpdateVisualsForPhase(currentPhase);
        OnPhaseChanged?.Invoke(currentPhase);
    }

    private void Update()
    {
        if (isDead) return;
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Activa la música del jefe la primera vez que detecta al jugador
            if (!hasTriggeredBossMusic)
            {
                hasTriggeredBossMusic = true;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBossMusic();
                }
            }

            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();

                if (canShootProjectiles && Time.time >= nextShootTime)
                {
                    ShootAtPlayer();
                }
            }
            else
            {
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                }
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
    }

    private void ShootAtPlayer()
    {
        nextShootTime = Time.time + shootCooldown;

        if (projectileFactory == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up * 1.5f + transform.forward * 1.0f;
        Vector3 targetPos = playerTransform.position + Vector3.up * 1.0f;
        Quaternion rotation = Quaternion.LookRotation(targetPos - spawnPos);

        projectileFactory.CreateProjectile(currentPhase, spawnPos, rotation);

        if (shootSound != null)
        {
            PlaySound(shootSound);
        }
        else
        {
            PlaySound(attackSound);
        }

        Debug.Log($"[Jefe - Disparo] Disparó proyectil de {currentPhase} hacia el jugador.");
    }

    private void AttackPlayer()
    {
        nextAttackTime = Time.time + attackCooldown;

        PlaySound(attackSound);

        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        Debug.Log($"[Jefe] ¡Ataque exitoso! Daño infligido: {attackDamage}");
    }

    /// <summary>
    /// Recibe daño si el proyectil coincide con la debilidad de la fase elemental actual.
    /// Fases y debilidades:
    /// - Fase 0: Fire -> Vulnerable a Acid
    /// - Fase 1: Ice -> Vulnerable a Fire
    /// - Fase 2: Electricity -> Vulnerable a Ice
    /// - Fase 3: Acid -> Vulnerable a Electricity
    /// </summary>
    /// <returns>True si el daño fue efectivo, False si fue inmune.</returns>
    public bool TakeDamage(ElementType bulletElement, int damage = 5)
    {
        if (isDead) return false;

        ElementType requiredWeakness = GetWeaknessFor(currentPhase);

        if (bulletElement == requiredWeakness)
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;

            PlaySound(hurtSound);

            Debug.Log($"[Jefe - Daño Efectivo] Impactado por {bulletElement}. Vida: {currentHealth}/{maxHealth}");
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                CheckPhaseTransition();
            }

            return true;
        }
        else
        {
            Debug.Log($"[Jefe - Inmune] La fase actual es {currentPhase}. Requiere {requiredWeakness}, pero recibió {bulletElement}.");
            return false;
        }
    }

    private ElementType GetWeaknessFor(ElementType phase)
    {
        switch (phase)
        {
            case ElementType.Fire:
                return ElementType.Acid;
            case ElementType.Ice:
                return ElementType.Fire;
            case ElementType.Electricity:
                return ElementType.Ice;
            case ElementType.Acid:
                return ElementType.Electricity;
            default:
                return ElementType.Fire;
        }
    }

    private void CheckPhaseTransition()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        // Fases:
        // > 75% -> Fase 0: Fire
        // 50% - 75% -> Fase 1: Ice
        // 25% - 50% -> Fase 2: Electricity
        // <= 25% -> Fase 3: Acid
        int targetPhaseIndex = currentPhaseIndex;

        if (healthRatio <= 0.25f)
        {
            targetPhaseIndex = 3;
        }
        else if (healthRatio <= 0.50f)
        {
            targetPhaseIndex = 2;
        }
        else if (healthRatio <= 0.75f)
        {
            targetPhaseIndex = 1;
        }
        else
        {
            targetPhaseIndex = 0;
        }

        // Si avanzó a una fase superior, ejecutamos la transición solo una vez
        if (targetPhaseIndex > currentPhaseIndex)
        {
            currentPhaseIndex = targetPhaseIndex;
            currentPhase = GetElementFromPhaseIndex(currentPhaseIndex);

            UpdateVisualsForPhase(currentPhase);
            PlaySound(phaseChangeSound);

            Debug.Log($"[Jefe - Cambio de Fase] ¡El jefe cambia a fase {currentPhase} (Vulnerable a {GetWeaknessFor(currentPhase)})!");
            OnPhaseChanged?.Invoke(currentPhase);
        }
    }

    private void UpdateVisualsForPhase(ElementType phase)
    {
        Material targetMat = null;
        Color targetColor = fireColor;

        switch (phase)
        {
            case ElementType.Fire:
                targetMat = fireMaterial;
                targetColor = fireColor;
                break;
            case ElementType.Ice:
                targetMat = iceMaterial;
                targetColor = iceColor;
                break;
            case ElementType.Electricity:
                targetMat = electricityMaterial;
                targetColor = electricityColor;
                break;
            case ElementType.Acid:
                targetMat = acidMaterial;
                targetColor = acidColor;
                break;
        }

        if (bossRenderer != null)
        {
            if (targetMat != null)
            {
                bossRenderer.material = targetMat;
            }
            else if (bossRenderer.material != null)
            {
                bossRenderer.material.color = targetColor;
            }
        }

        if (bossLight != null)
        {
            bossLight.color = targetColor;
        }
    }

    private ElementType GetElementFromPhaseIndex(int index)
    {
        switch (index)
        {
            case 0: return ElementType.Fire;
            case 1: return ElementType.Ice;
            case 2: return ElementType.Electricity;
            case 3: return ElementType.Acid;
            default: return ElementType.Fire;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        PlaySound(deathSound);

        Debug.Log("[Jefe - Derrotado] El jefe ha caído.");
        OnBossDeath?.Invoke();

        // Desactivamos el collider para que no bloquee proyectiles ni ataque tras morir
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Activa la victoria en el Game Loop
        if (gameOverController != null)
        {
            gameOverController.TriggerVictory();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && bossAudioSource != null)
        {
            bossAudioSource.PlayOneShot(clip, bossVolume);
        }
    }
}
