using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CustomGrabLock : XRGrabInteractable
{
    private bool isBeingHeld = false;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        Debug.Log("Selected Enter");

        isBeingHeld = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        Debug.Log("Selected Exit");

        isBeingHeld = false;
    }

    void FixedUpdate()
    {
        if (isBeingHeld)
        {
            // Freeze position manually
            transform.position = attachTransform.position;

            // Allow rotation only on Z
            Vector3 euler = attachTransform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, euler.z);
        }
    }
}