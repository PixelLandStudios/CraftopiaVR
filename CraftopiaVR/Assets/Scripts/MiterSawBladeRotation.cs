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

    [Header("Audio Reference")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip SawRunningClip;
    [SerializeField] AudioClip SawEndClip;

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

        audioSource.clip = SawRunningClip;
        audioSource.Play();

        Debug.Log("Saw Grabbed");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Stop rotating when released
        isGrabbed = false;

        audioSource.clip = SawEndClip;
        audioSource.Play();

        Debug.Log("Saw Released");
    }

    void Update()
    {
        if (isGrabbed)
        {
            currentRotationSpeed = 2000;

            //// Accelerate the blade rotation
            //if (currentRotationSpeed < maxRotationSpeed)
            //{
            //    currentRotationSpeed += acceleration * Time.deltaTime;
            //    currentRotationSpeed = Mathf.Min(currentRotationSpeed, maxRotationSpeed);
            //}
        }
        else
        {
            currentRotationSpeed = 0;

            //// Decelerate the blade rotation
            //if (currentRotationSpeed > 0)
            //{
            //    currentRotationSpeed -= 1000 * Time.deltaTime;
            //    currentRotationSpeed = Mathf.Max(currentRotationSpeed, 0);
            //}
        }

        // Rotate the blade around its Z-axis
        if (bladeTransform != null)
        {
            bladeTransform.Rotate(0, 0, currentRotationSpeed);
        }
    }
}