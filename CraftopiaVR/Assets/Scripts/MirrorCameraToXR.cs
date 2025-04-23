using UnityEngine;

public class MirrorCameraToXR : MonoBehaviour
{
    public Transform xrCamera; // Assign XR Camera in inspector
    public float customFOV = 90f; // Your desired field of view

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = customFOV;
        }
    }

    void LateUpdate()
    {
        if (xrCamera != null)
        {
            transform.position = xrCamera.position;
            transform.rotation = xrCamera.rotation;
        }

        // Lock the FOV to prevent override
        if (cam.fieldOfView != customFOV)
        {
            cam.fieldOfView = customFOV;
        }
    }
}
