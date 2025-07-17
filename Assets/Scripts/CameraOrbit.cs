using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    [Tooltip("The object to orbit around")]
    public Transform target;

    [Tooltip("Distance from the target")]
    public float distance = 5.0f;

    [Tooltip("Mouse sensitivity (X = horizontal, Y = vertical)")]
    public Vector2 sensitivity = new(0.1f, 0.1f);

    [Tooltip("Min/max vertical angle")]
    public float minYAngle = -20f;
    public float maxYAngle = 80f;

    [Tooltip("Smoothing factor for camera inertia (higher = snappier)")]
    public float inertia = 5f;

    private float yaw;
    private float pitch;
    private float targetYaw;
    private float targetPitch;

    void Start()
    {
        if (target == null)
            Debug.LogWarning($"{typeof(CameraOrbit)}: no target assigned.");

        Vector3 angles = transform.eulerAngles;
        yaw = targetYaw = angles.y;
        pitch = targetPitch = angles.x;
    }

    void Update()
    {
        if (target == null)
            return;

        var mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            targetYaw   += delta.x * sensitivity.x;
            targetPitch -= delta.y * sensitivity.y;
            targetPitch  = Mathf.Clamp(targetPitch, minYAngle, maxYAngle);
        }

        // Smoothly interpolate towards target angles
        yaw   = Mathf.Lerp(yaw,   targetYaw,   inertia * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, targetPitch, inertia * Time.deltaTime);

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset      = rotation * new Vector3(0f, 0f, -distance);
        transform.position  = target.position + offset;
        transform.rotation  = rotation;
    }
}