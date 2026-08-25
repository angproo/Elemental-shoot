using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Materiales")]
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material iceMaterial;

    private ElementType currentElement = ElementType.Fire;

    private void Update()
    {
        HandleElementInput();

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void HandleElementInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentElement = ElementType.Fire;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentElement = ElementType.Ice;
        }
    }

    private void Shoot()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetElement(currentElement);

            if (currentElement == ElementType.Fire)
            {
                projectile.SetElementMaterial(fireMaterial);
            }
            else
            {
                projectile.SetElementMaterial(iceMaterial);
            }
        }
    }
}