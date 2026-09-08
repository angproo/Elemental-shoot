using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int health = 3;

    [Header("Debilidad")]
    [SerializeField] private ElementType weakness;

    public void TakeDamage(ElementType bulletElement)
    {
        if (bulletElement == weakness)
        {
            health--;

            Debug.Log("Daño recibido");

            if (health <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("Este enemigo es inmune a " + bulletElement);
        }
    }

    private void Die()
    {
        Debug.Log("Enemigo derrotado");

        Destroy(gameObject);
    }
}