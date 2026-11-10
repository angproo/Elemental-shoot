using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders de UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Panel de Opciones (Opcional)")]
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        InitializeSliders();
    }

    private void OnEnable()
    {
        // Al reabrir el panel, sincronizar sliders con los valores guardados
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        AudioManager audioManager = AudioManager.Instance != null ? AudioManager.Instance : FindFirstObjectByType<AudioManager>();

        if (audioManager != null)
        {
            if (masterSlider != null)
            {
                masterSlider.value = audioManager.MasterVolume;
                masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
                masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            }

            if (musicSlider != null)
            {
                musicSlider.value = audioManager.MusicVolume;
                musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
                musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = audioManager.SfxVolume;
                sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
                sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
            }
        }
    }

    public void OnMasterSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }

    public void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    public void OnSfxSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxVolume(value);
        }
    }

    /// <summary>
    /// Abre el panel de opciones de sonido.
    /// </summary>
    public void OpenOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            InitializeSliders();
        }
    }

    /// <summary>
    /// Cierra el panel de opciones de sonido.
    /// </summary>
    public void CloseOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }
}
