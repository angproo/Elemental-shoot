using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Configuración de Proyectil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private ElementType elementType;

    [Header("Configuración de Audio de Impacto")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip effectiveHitSound;
    [Range(0f, 1f)] [SerializeField] private float impactVolume = 1f;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    public void SetElement(ElementType element)
    {
        elementType = element;
    }

    public void SetElementMaterial(Material material)
    {
        if (meshRenderer != null && material != null)
        {
            meshRenderer.material = material;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool hasHitEnemy = false;
        bool isEffective = false;

        // Intenta dañar a un enemigo si el objeto colisionado tiene el script Enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            hasHitEnemy = true;
            isEffective = enemy.TakeDamage(elementType);
        }

        // Intenta dañar al jefe final si el objeto colisionado tiene el script BossController
        BossController boss = collision.gameObject.GetComponent<BossController>();
        if (boss != null)
        {
            hasHitEnemy = true;
            isEffective = boss.TakeDamage(elementType);
        }

        // Intenta dañar un cerrojo elemental si el objeto colisionado tiene el script ElementalLock
        ElementalLock lockObject = collision.gameObject.GetComponent<ElementalLock>();
        if (lockObject != null)
        {
            lockObject.TakeDamage(elementType);
        }

        // Reproduce el sonido de impacto si corresponde
        if (hasHitEnemy)
        {
            if (isEffective && effectiveHitSound != null)
            {
                AudioSource.PlayClipAtPoint(effectiveHitSound, transform.position, impactVolume);
            }
            else if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position, impactVolume);
            }
        }
        else if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, impactVolume);
        }

        Destroy(gameObject);
    }
}