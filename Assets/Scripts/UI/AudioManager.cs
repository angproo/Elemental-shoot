using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio (AudioSource)")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Música Ambiental y de Combate")]
    [SerializeField] private AudioClip ambientMusic;
    [SerializeField] private AudioClip combatMusic;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.7f;

    [Header("Música / Sonidos de Fin de Partida")]
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;
    [Range(0f, 1f)] [SerializeField] private float endMusicVolume = 0.8f;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Configuración o creación automática de AudioSources si no fueron asignados
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // La música de fondo debe estar en bucle y escuchar sin verse afectada por Time.timeScale = 0
        musicAudioSource.loop = true;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.ignoreListenerPause = true;

        // La fuente de efectos/fin de partida ignora la pausa de Time.timeScale
        sfxAudioSource.loop = false;
        sfxAudioSource.volume = endMusicVolume;
        sfxAudioSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        PlayAmbientMusic();
    }

    /// <summary>
    /// Inicia la música ambiental sin reiniciar si ya se está reproduciendo.
    /// </summary>
    public void PlayAmbientMusic()
    {
        if (isGameOver) return;
        PlayMusic(ambientMusic);
    }

    /// <summary>
    /// Cambia a la música de combate opcional si está asignada.
    /// </summary>
    public void PlayCombatMusic()
    {
        if (isGameOver) return;
        if (combatMusic != null)
        {
            PlayMusic(combatMusic);
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
    /// Detiene la música ambiental y reproduce el sonido o música de victoria sin solaparse con derrota.
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
            sfxAudioSource.PlayOneShot(victoryMusic, endMusicVolume);
        }
    }

    /// <summary>
    /// Detiene la música ambiental y reproduce el sonido o música de derrota sin solaparse con victoria.
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
            sfxAudioSource.PlayOneShot(defeatMusic, endMusicVolume);
        }
    }

    /// <summary>
    /// Ajusta el volumen de música en tiempo de ejecución o desde Inspector.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }
    }
}
