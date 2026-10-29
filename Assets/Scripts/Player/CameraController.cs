using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] private float mouseSensitivity = 100.0f;

    [Header("Referencias")]
    [SerializeField] private Transform playerBody;

    [Header("Límites de Mirada Vertical")]
    [SerializeField] private float minPitch = -85.0f;
    [SerializeField] private float maxPitch = 85.0f;

    private float xRotation = 0.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
