using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Required for XR components
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ScrewStickOnTouch : MonoBehaviour
{
    public string woodTag = "WoodPlank"; // Tag for the wood plank

    private bool isStuck = false;
    private Vector3 originalScale; // Store the screw's original scale

    private void Start()
    {
        // Store the screw's original scale at the start
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the screw is not already stuck and if the collision is with a wood plank
        if (!isStuck && collision.gameObject.CompareTag(woodTag))
        {
            StickToWood(collision);
        }
    }

    private void StickToWood(Collision collision)
    {
        // Remove the XRGrabInteractable component
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            Destroy(grabInteractable);
        }

        // Remove the XRGeneralGrabTransformer component
        XRGeneralGrabTransformer grabTransformer = GetComponent<XRGeneralGrabTransformer>();
        if (grabTransformer != null)
        {
            Destroy(grabTransformer);
        }

        // Remove the Rigidbody component
        Rigidbody screwRigidbody = GetComponent<Rigidbody>();
        if (screwRigidbody != null)
        {
            Destroy(screwRigidbody);
        }

        // Get the contact normal
        ContactPoint contact = collision.contacts[0];
        Vector3 contactNormal = contact.normal;

        // Parent the screw to the wood plank
        transform.parent = collision.transform;

        // Align the screw with the surface normal
        transform.up = contactNormal;

        // Adjust the screw's local scale to counteract the parent's scale
        Vector3 parentScale = collision.transform.localScale;
        transform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );

        // Mark the screw as stuck
        isStuck = true;
    }
}