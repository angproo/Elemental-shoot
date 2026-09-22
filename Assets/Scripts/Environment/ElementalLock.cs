using UnityEngine;

public class ElementalLock : MonoBehaviour
{
    [Header("Configuración del Cerrojo")]
    [SerializeField] private ElementType requiredElement = ElementType.Fire; // Elemento necesario para activarlo/destruirlo
    [SerializeField] private int hitsRequired = 1;                           // Cantidad de impactos necesarios

    [Header("Objeto a Desbloquear / Puerta")]
    [SerializeField] private GameObject doorObject;                          // Puerta u objeto que se destruirá o abrirá

    private int currentHits = 0;

    public void TakeDamage(ElementType bulletElement)
    {
        // Solo responde al elemento requerido
        if (bulletElement == requiredElement)
        {
            currentHits++;
            Debug.Log($"[Cerrojo Elemental] ¡Impacto correcto ({bulletElement}) en {gameObject.name}! ({currentHits}/{hitsRequired})");

            if (currentHits >= hitsRequired)
            {
                UnlockDoor();
            }
        }
        else
        {
            Debug.Log($"[Cerrojo Elemental] {gameObject.name} requiere el elemento {requiredElement}. El proyectil era de {bulletElement}.");
        }
    }

    private void UnlockDoor()
    {
        Debug.Log($"[Cerrojo Elemental] Cerrojo {gameObject.name} destruido. Abrir puerta: {(doorObject != null ? doorObject.name : "Sin asignar")}");

        // Si se asignó una puerta, se desactiva/destruye para dar paso al jugador
        if (doorObject != null)
        {
            Destroy(doorObject);
        }

        // Destruye el cerrojo mismo
        Destroy(gameObject);
    }
}
