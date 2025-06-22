using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodeController : MonoBehaviour
{
    // Drone parts from your screenshot
    public Transform Battery_Parent;
    public Transform FL_Motor_Parent;
    public Transform Cam_Parent;
    public Transform RL_Motor_Parent;
    public Transform FR_Motor_Parent;
    public Transform RR_Motor_Parent;
    public Transform FLLEDs;
    public Transform PR_LEDs;
    public Transform RR_LEDs;
    public Transform RL_LEDs;

    private Vector3[] originalPositions;
    private Vector3[] explodedPositions;
    private Transform[] interactableObjects;

    private bool isExploded = false;
    public float moveSpeed = 2.0f;

    private Transform selectedObject = null;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Assign all interactable drone parts
        interactableObjects = new Transform[] {
            Battery_Parent,
            FL_Motor_Parent,
            Cam_Parent,
            RL_Motor_Parent,
            FR_Motor_Parent,
            RR_Motor_Parent,
            FLLEDs,
            PR_LEDs,
            RR_LEDs,
            RL_LEDs
        };

        originalPositions = new Vector3[interactableObjects.Length];
        explodedPositions = new Vector3[interactableObjects.Length];

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (interactableObjects[i] != null)
            {
                originalPositions[i] = interactableObjects[i].localPosition;
                explodedPositions[i] = originalPositions[i] + GetExplodedOffset(interactableObjects[i]);
            }
        }
    }

    Vector3 GetExplodedOffset(Transform obj)
    {
        // Custom directional explosion for each component
        if (obj == Battery_Parent) return new Vector3(0, -1.5f, 0); // Battery moves straight down

        if (obj == FL_Motor_Parent) return new Vector3(1.5f, 0, 0); // Front left motor moves right
        if (obj == FR_Motor_Parent) return new Vector3(1.5f, 0, 0); // Front right motor moves right
        if (obj == RL_Motor_Parent) return new Vector3(-1.5f, 0, 0); // Rear left motor moves left
        if (obj == RR_Motor_Parent) return new Vector3(-1.5f, 0, 0); // Rear right motor moves left

        if (obj == Cam_Parent) return new Vector3(0, 0, 2.0f); // Camera moves straight forward
        if (obj == FLLEDs) return new Vector3(0, 0, 1.5f); // Front left LEDs move forward
        if (obj == PR_LEDs) return new Vector3(0, 0, 1.5f); // Front right LEDs move forward
        if (obj == RR_LEDs) return new Vector3(0, 0, -1.5f); // Rear right LEDs move backward
        if (obj == RL_LEDs) return new Vector3(0, 0, -1.5f); // Rear left LEDs move backward

        return Vector3.zero;
    }

    public void ToggleExplode()
    {
        StopAllCoroutines();

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (interactableObjects[i] != null)
            {
                Vector3 targetPos = isExploded ? originalPositions[i] : explodedPositions[i];
                StartCoroutine(MoveToPosition(interactableObjects[i], targetPos));
            }
        }

        isExploded = !isExploded;
    }

    IEnumerator MoveToPosition(Transform obj, Vector3 targetPosition)
    {
        float timeElapsed = 0;
        Vector3 startPosition = obj.localPosition;

        while (timeElapsed < 1)
        {
            obj.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * moveSpeed;
            yield return null;
        }

        obj.localPosition = targetPosition;
    }

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame && selectedObject == null)
        {
            Ray ray = mainCamera.ScreenPointToRay(touch.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (IsInteractable(hit.transform))
                {
                    selectedObject = hit.transform;
                    offset = selectedObject.position - GetTouchWorldPosition();
                    Debug.Log($"Touched on: {selectedObject.name}");
                }
            }
        }

        if (touch.press.isPressed && selectedObject != null)
        {
            Vector3 touchPos = GetTouchWorldPosition() + offset;
            selectedObject.position = touchPos;
        }

        if (touch.press.wasReleasedThisFrame && selectedObject != null)
        {
            int index = System.Array.IndexOf(interactableObjects, selectedObject);
            if (index >= 0)
            {
                Vector3 targetPos = isExploded ? explodedPositions[index] : originalPositions[index];
                StartCoroutine(MoveToPosition(selectedObject, targetPos));
            }
            selectedObject = null;
        }
    }

    Vector3 GetTouchWorldPosition()
    {
        Vector3 touchPoint = Touchscreen.current.primaryTouch.position.ReadValue();
        touchPoint.z = 10f; // Adjust based on object's distance from the camera
        return mainCamera.ScreenToWorldPoint(touchPoint);
    }

    bool IsInteractable(Transform obj)
    {
        return System.Array.IndexOf(interactableObjects, obj) >= 0;
    }
}