using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MiterSawBladeRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxRotationSpeed = 360f; // Maximum rotation speed in degrees per second
    public float acceleration = 90f; // Acceleration in degrees per second squared
    public float deceleration = 180f; // Deceleration in degrees per second squared

    [Header("Blade Reference")]
    [SerializeField] private Transform bladeTransform; // Serialized field for the blade

    private XRGrabInteractable grabInteractable;
    private float currentRotationSpeed = 0f;
    private bool isGrabbed = false;

    void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Add listeners for grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Check if the blade Transform is assigned
        if (bladeTransform == null)
        {
            Debug.LogError("Blade Transform is not assigned! Please assign the blade in the Inspector.");
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Start rotating when grabbed
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Stop rotating when released
        isGrabbed = false;
    }

    void Update()
    {
        if (isGrabbed)
        {
            // Accelerate the blade rotation
            if (currentRotationSpeed < maxRotationSpeed)
            {
                currentRotationSpeed += acceleration * Time.deltaTime;
                currentRotationSpeed = Mathf.Min(currentRotationSpeed, maxRotationSpeed);
            }
        }
        else
        {
            // Decelerate the blade rotation
            if (currentRotationSpeed > 0)
            {
                currentRotationSpeed -= deceleration * Time.deltaTime;
                currentRotationSpeed = Mathf.Max(currentRotationSpeed, 0);
            }
        }

        // Rotate the blade around its Z-axis
        if (bladeTransform != null)
        {
            bladeTransform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
        }
    }
}