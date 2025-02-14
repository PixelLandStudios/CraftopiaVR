using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Add this namespace

public class ZAxisRotationConstraint : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Add listeners for grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Store the initial rotation and position when grabbed
        initialRotation = transform.rotation;
        initialPosition = transform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Reset to the initial rotation and position when released
        transform.rotation = initialRotation;
        transform.position = initialPosition;
    }

    void Update()
    {
        // Check if the object is currently grabbed
        if (grabInteractable.isSelected)
        {
            // Freeze the position to the initial position
            transform.position = initialPosition;

            // Get the current rotation
            Vector3 currentRotation = transform.rotation.eulerAngles;

            // Constrain rotation to Z-axis only
            transform.rotation = Quaternion.Euler(0, 90, currentRotation.z);
        }
    }
}