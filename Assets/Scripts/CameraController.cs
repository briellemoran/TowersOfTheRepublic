using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 500f; 
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Zoom Settings")]
    public float zoomSensitivity = 5f;
    public float minFOV = 15f;
    public float maxFOV = 90f;
    public float zoomSmoothness = 10f;

    private float currentYaw;
    private float currentPitch;
    private float targetFOV;
    private Camera cam;
    private bool isRotating = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController must be attached to a Camera object.");
            enabled = false;
            return;
        }

        // Initialize angles from current rotation
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;
        // Normalize pitch to -180, 180 range
        if (currentPitch > 180f) currentPitch -= 360f;

        targetFOV = cam.fieldOfView;
    }

    void LateUpdate() // Use LateUpdate for camera to ensure it moves after other logic
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Only start rotating if we didn't click on a UI element
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                isRotating = false;
            }
            else
            {
                isRotating = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f)
            {
                currentYaw += mouseX * rotationSpeed * Time.unscaledDeltaTime;
                currentPitch -= mouseY * rotationSpeed * Time.unscaledDeltaTime;
                
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

                transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            }
        }
    }

    private void HandleZoom()
    {
        // Don't zoom if mouse is over UI (optional, but often preferred)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // We can still allow zooming if desired, but usually it's better to block it 
            // if the user is interacting with a scrollable UI list.
            // For now, let's allow it unless the user complains.
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            // Adjust target FOV based on scroll
            targetFOV = Mathf.Clamp(targetFOV - scroll * zoomSensitivity * 10f, minFOV, maxFOV);
        }

        // Smoothly interpolate FOV
        if (Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.unscaledDeltaTime * zoomSmoothness);
        }
        else
        {
            cam.fieldOfView = targetFOV;
        }
    }
}
