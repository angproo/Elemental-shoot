using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Configuración de Proyectil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private ElementType elementType;

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
        // Intenta dañar a un enemigo si el objeto colisionado tiene el script Enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(elementType);
        }

        // Intenta dañar un cerrojo elemental si el objeto colisionado tiene el script ElementalLock
        ElementalLock lockObject = collision.gameObject.GetComponent<ElementalLock>();
        if (lockObject != null)
        {
            lockObject.TakeDamage(elementType);
        }

        Destroy(gameObject);
    }
}