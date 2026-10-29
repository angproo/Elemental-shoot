using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    [SerializeField] private ProjectileFactory projectileFactory;
    [SerializeField] private Transform firePoint;

    [Header("Configuración de Disparo y Munición")]
    [SerializeField] private float fireRate = 0.2f;       // Tiempo mínimo en segundos entre disparos
    [SerializeField] private int maxAmmo = 30;             // Capacidad máxima del cargador
    [SerializeField] private float reloadTime = 1.5f;      // Tiempo en segundos que tarda la recarga

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptyAmmoSound;
    [Range(0f, 1f)] [SerializeField] private float weaponVolume = 1f;

    // Estado interno del arma
    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    private ElementType currentElement = ElementType.Fire;

    // Propiedades públicas para consultar el estado del arma
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public ElementType CurrentElement => currentElement;

    private void Awake()
    {
        // Si no está asignada la fábrica por Inspector, la busca en el mismo GameObject
        if (projectileFactory == null)
        {
            projectileFactory = GetComponent<ProjectileFactory>();
        }

        // Si no está asignado el AudioSource por Inspector, busca o crea uno en este GameObject
        if (weaponAudioSource == null)
        {
            weaponAudioSource = GetComponent<AudioSource>();
            if (weaponAudioSource == null)
            {
                weaponAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        // Si el juego está pausado o en pantalla de fin de juego, no procesar acciones del arma
        if (Time.timeScale == 0f) return;

        if (isReloading) return;

        // Selección de elementos con teclas 1, 2, 3 y 4
        HandleElementInput();

        // Recarga manual con tecla R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        // Disparo con clic izquierdo
        if (Input.GetButtonDown("Fire1"))
        {
            if (currentAmmo <= 0)
            {
                PlaySound(emptyAmmoSound);
                StartCoroutine(ReloadRoutine());
                return;
            }

            if (Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
    }

    private void HandleElementInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentElement = ElementType.Fire;
            Debug.Log("Elemento activo: FUEGO (1)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentElement = ElementType.Ice;
            Debug.Log("Elemento activo: HIELO (2)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentElement = ElementType.Electricity;
            Debug.Log("Elemento activo: ELECTRICIDAD (3)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentElement = ElementType.Acid;
            Debug.Log("Elemento activo: ÁCIDO (4)");
        }
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        PlaySound(shootSound);

        // Delega la creación de la bala a la fábrica (ProjectileFactory)
        if (projectileFactory != null && firePoint != null)
        {
            projectileFactory.CreateProjectile(currentElement, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("[WeaponController] Falta asignar ProjectileFactory o FirePoint.");
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Recargando arma...");

        PlaySound(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Arma recargada: " + currentAmmo + "/" + maxAmmo);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && weaponAudioSource != null)
        {
            weaponAudioSource.PlayOneShot(clip, weaponVolume);
        }
    }
}