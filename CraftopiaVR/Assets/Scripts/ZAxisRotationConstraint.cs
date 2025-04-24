using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ZAxisRotationConstraint : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Optional: reset everything on release
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    void LateUpdate()
    {
        if (grabInteractable.isSelected)
        {
            // Re-lock the position hard every frame
            transform.position = initialPosition;

            // Allow only Z-axis rotation, with a fixed base rotation
            float z = transform.localEulerAngles.z;
            transform.localRotation = Quaternion.Euler(0f, 90f, z); // 90f assumes Y is forward
        }
    }
}