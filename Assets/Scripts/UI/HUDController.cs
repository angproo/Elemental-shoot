using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Referencias a Scripts del Jugador")]
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Elementos de la Interfaz (UI)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI elementNameText;
    [SerializeField] private TextMeshProUGUI reloadIndicatorText;
    [SerializeField] private Image elementIconImage;
    [SerializeField] private Image crosshairImage;

    [Header("Colores por Elemento")]
    [SerializeField] private Color fireColor = new Color(1f, 0.3f, 0.1f);
    [SerializeField] private Color iceColor = new Color(0.2f, 0.7f, 1f);
    [SerializeField] private Color electricityColor = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private Color acidColor = new Color(0.3f, 1f, 0.2f);

    private void Start()
    {
        // Si no están asignados por Inspector, intenta buscarlos en la escena
        if (weaponController == null)
        {
            weaponController = FindFirstObjectByType<WeaponController>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateAmmoAndElementUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = $"HP: {playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";
        }
    }

    private void UpdateAmmoAndElementUI()
    {
        if (weaponController == null) return;

        // Actualiza el texto de munición (ej: 30 / 30)
        if (ammoText != null)
        {
            ammoText.text = $"{weaponController.CurrentAmmo} / {weaponController.MaxAmmo}";
        }

        // Muestra u oculta el indicador de recarga
        if (reloadIndicatorText != null)
        {
            reloadIndicatorText.gameObject.SetActive(weaponController.IsReloading);
            if (weaponController.IsReloading)
            {
                reloadIndicatorText.text = "RECARGANDO...";
            }
        }

        // Actualiza el elemento actual en la UI (Texto e Imagen/Color)
        UpdateElementDisplay(weaponController.CurrentElement);
    }

    private void UpdateElementDisplay(ElementType currentElement)
    {
        string nameStr = "";
        Color elementColor = Color.white;

        switch (currentElement)
        {
            case ElementType.Fire:
                nameStr = "FUEGO";
                elementColor = fireColor;
                break;
            case ElementType.Ice:
                nameStr = "HIELO";
                elementColor = iceColor;
                break;
            case ElementType.Electricity:
                nameStr = "ELECTRICIDAD";
                elementColor = electricityColor;
                break;
            case ElementType.Acid:
                nameStr = "ÁCIDO";
                elementColor = acidColor;
                break;
        }

        if (elementNameText != null)
        {
            elementNameText.text = nameStr;
            elementNameText.color = elementColor;
        }

        if (elementIconImage != null)
        {
            elementIconImage.color = elementColor;
        }
    }
}
