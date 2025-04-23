using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GlueBottleController : MonoBehaviour
{
    public GameObject glueDropPrefab; // Assign the glue drop prefab in the Inspector
    public Transform spawnPoint; // Assign a spawn point (e.g., the nozzle of the glue bottle)

    [SerializeField]
    Animation triggerAnimation;

    [SerializeField]
    AudioSource audioSource;

    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;

    void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to the select and activate events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.activated.AddListener(OnTriggerPress);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void OnTriggerPress(ActivateEventArgs args)
    {
        if (isGrabbed && glueDropPrefab != null && spawnPoint != null)
        {
            // Spawn the glue drop at the spawn point
            GameObject glueDrop = Instantiate(glueDropPrefab, spawnPoint.position, spawnPoint.rotation);

            triggerAnimation.Play();
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
        grabInteractable.activated.RemoveListener(OnTriggerPress);
    }
}