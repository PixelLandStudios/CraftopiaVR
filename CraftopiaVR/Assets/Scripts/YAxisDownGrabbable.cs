using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class YAxisDownGrabbable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Vector3 initialLocalPosition; // Stores the initial local position relative to the parent
    private float initialYPosition; // Stores the initial Y position in world space

    private void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Add listeners for grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Store the initial local position relative to the parent
        initialLocalPosition = transform.localPosition;

        // Store the initial Y position in world space
        initialYPosition = transform.position.y;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Store the initial Y position in world space when grabbed
        initialYPosition = transform.position.y;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Reset the local position to the initial local position when released
        transform.localPosition = initialLocalPosition;
    }

    private void Update()
    {
        if (grabInteractable.isSelected)
        {
            // Get the current world Y position
            float currentY = transform.position.y;

            // If the object is moved upward, clamp it back to the initial Y position
            if (currentY > initialYPosition)
            {
                transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);
            }
        }
    }
}