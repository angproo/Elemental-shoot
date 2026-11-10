using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio (AudioSource)")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Clips de Música")]
    [SerializeField] private AudioClip ambientMusic;
    [SerializeField] private AudioClip bossMusic;

    [Header("Música / Sonidos de Fin de Partida")]
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("Configuración de Volúmenes por Defecto")]
    [Range(0f, 1f)] [SerializeField] private float defaultMasterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float defaultMusicVolume = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float defaultSfxVolume = 0.8f;

    private float masterVolume = 1f;
    private float musicVolume = 0.7f;
    private float sfxVolume = 0.8f;

    private bool isGameOver = false;

    // Constantes PlayerPrefs para persistencia
    private const string PREF_MASTER = "Audio_MasterVolume";
    private const string PREF_MUSIC = "Audio_MusicVolume";
    private const string PREF_SFX = "Audio_SfxVolume";

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Persistir entre escenas (por ejemplo entre MainMenu y GameScene)
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // Configuración o creación automática de AudioSources si no fueron asignados
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }

        musicAudioSource.loop = true;
        musicAudioSource.ignoreListenerPause = true;

        sfxAudioSource.loop = false;
        sfxAudioSource.ignoreListenerPause = true;

        LoadSavedVolumes();
        ApplyVolumes();
    }

    private void Start()
    {
        // Si no está sonando música, inicia la ambiental
        if (!musicAudioSource.isPlaying && ambientMusic != null)
        {
            PlayAmbientMusic();
        }
    }

    private void LoadSavedVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, defaultMasterVolume);
        musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC, defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, defaultSfxVolume);
    }

    private void ApplyVolumes()
    {
        AudioListener.volume = masterVolume;

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Cambia a la música ambiental si no está en Game Over.
    /// </summary>
    public void PlayAmbientMusic()
    {
        if (isGameOver) return;
        PlayMusic(ambientMusic);
    }

    /// <summary>
    /// Cambia a la música del jefe final (combate).
    /// </summary>
    public void PlayBossMusic()
    {
        if (isGameOver) return;
        if (bossMusic != null)
        {
            PlayMusic(bossMusic);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicAudioSource.clip == clip && musicAudioSource.isPlaying) return;

        musicAudioSource.clip = clip;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.Play();
    }

    /// <summary>
    /// Detiene la música ambiental y reproduce el sonido o música de victoria.
    /// </summary>
    public void PlayVictory()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }

        if (victoryMusic != null && sfxAudioSource != null)
        {
            sfxAudioSource.Stop();
            sfxAudioSource.PlayOneShot(victoryMusic, sfxVolume);
        }
    }

    /// <summary>
    /// Detiene la música y reproduce el sonido o música de derrota.
    /// </summary>
    public void PlayDefeat()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }

        if (defeatMusic != null && sfxAudioSource != null)
        {
            sfxAudioSource.Stop();
            sfxAudioSource.PlayOneShot(defeatMusic, sfxVolume);
        }
    }

    /// <summary>
    /// Reinicia el estado de GameOver al recargar una partida.
    /// </summary>
    public void ResetGameOverState()
    {
        isGameOver = false;
        if (sfxAudioSource != null)
        {
            sfxAudioSource.Stop();
        }
    }

    // --- MÉTODOS DE AJUSTE DE VOLUMEN (VINCULABLES A SLIDERS) ---

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat(PREF_MASTER, masterVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat(PREF_MUSIC, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat(PREF_SFX, sfxVolume);
        PlayerPrefs.Save();
    }
}
