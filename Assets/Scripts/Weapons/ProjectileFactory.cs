using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    [Header("Prefab Base")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Materiales por Elemento")]
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material iceMaterial;
    [SerializeField] private Material electricityMaterial;
    [SerializeField] private Material acidMaterial;

    /// <summary>
    /// Crea un proyectil en la posición y rotación especificadas, aplicando el elemento y su material correspondiente.
    /// </summary>
    public GameObject CreateProjectile(ElementType element, Vector3 position, Quaternion rotation)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ProjectileFactory] El projectilePrefab no está asignado en el Inspector.");
            return null;
        }

        // Instancia la bala física en el mundo
        GameObject bulletObject = Instantiate(projectilePrefab, position, rotation);

        // Aplica el tipo de elemento y el material correspondiente
        Projectile projectile = bulletObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetElement(element);
            Material elementMat = GetMaterialForElement(element);
            projectile.SetElementMaterial(elementMat);
        }

        return bulletObject;
    }

    /// <summary>
    /// Retorna el material correspondiente al tipo de elemento.
    /// </summary>
    private Material GetMaterialForElement(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return fireMaterial;
            case ElementType.Ice:
                return iceMaterial;
            case ElementType.Electricity:
                return electricityMaterial;
            case ElementType.Acid:
                return acidMaterial;
            default:
                return fireMaterial;
        }
    }
}
