using UnityEngine;

public class DroneRotationController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Drag the drone GameObject here")]
    public GameObject droneObject; // Assign your drone in the Inspector
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;
    [Tooltip("Rotation axis (e.g., Y-axis for spinning)")]
    public Vector3 rotationAxis = Vector3.up;

    private bool isRotating = false;

    void Update()
    {
        if (isRotating && droneObject != null)
        {
            droneObject.transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }

    // Call this from UI Button's OnClick event
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    // Alternative: Start rotation only while button is held down
    public void StartRotation() => isRotating = true;
    public void StopRotation() => isRotating = false;
}