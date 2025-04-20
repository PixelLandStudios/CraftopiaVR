using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class GlueScript : MonoBehaviour
{
    [SerializeField]
    GameObject firstPiece;

    [SerializeField]
    GameObject secondPiece;
    bool stopGluing;

    private bool preserveScale = true;
    private Vector3 originalScale;
    bool piecesReady = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (piecesReady)
        {
            InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButtonPressed) && aButtonPressed)
            {
                piecesReady = false;
                //StartCoroutine(CombineAfterDelay(0f)); // Or call CombineMeshes() directly if you want

                CombineMeshes();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stopGluing)
            return;

        if (other.CompareTag("WoodPlank"))
        {
            Debug.Log(other.name);

            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<Rigidbody>().useGravity = false;

            if (firstPiece == null)
            {
                firstPiece = other.gameObject;
                stopGluing = true;

                //Set this game object as a child to other.transform and keep it's transform not affected 
                ParentTo(other.transform);

                StartCoroutine(PauseGluing(2f));
            }
            else if (secondPiece == null)
            {
                secondPiece = other.gameObject;

                stopGluing = true;

                piecesReady = true;
                //StartCoroutine(CombineAfterDelay(1.5f));
            }
        }
    }

    public void ParentTo(Transform newParent)
    {
        // Preserve current world position, rotation, and scale
        Vector3 worldPosition = transform.position;
        Quaternion worldRotation = transform.rotation;
        Vector3 worldScale = transform.lossyScale;

        // Parent to the new transform
        transform.SetParent(newParent, false); // false means don't keep world position

        // Restore world position and rotation
        transform.position = worldPosition;
        transform.rotation = worldRotation;

        // Restore world scale
        Vector3 parentScale = newParent.lossyScale;
        transform.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }

    IEnumerator PauseGluing(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Combine the meshes after the delay
        stopGluing = false;
    }

    //IEnumerator CombineAfterDelay(float delay)
    //{
    //    Debug.Log($"Combining meshes in {delay} seconds...");

    //    // Wait for the specified delay
    //    yield return new WaitForSeconds(delay);

    //    // Combine the meshes after the delay
    //    //Combine();
    //    CombineMeshes();
    //}

    void CombineMeshes()
    {
        Debug.Log("First Piece: " + firstPiece.name + " , Second Piece: " + secondPiece.name);

        MeshFilter mesh1 = firstPiece.GetComponent<MeshFilter>();
        MeshFilter mesh2 = secondPiece.GetComponent<MeshFilter>();

        if (mesh1 == null || mesh2 == null)
        {
            Debug.LogError("Please assign both meshes in the Inspector.");
            return;
        }

        // Create a new GameObject to hold the combined mesh
        string randomNumber = Random.Range(1, 222).ToString();
        GameObject combinedObject = new GameObject("CombinedMesh" + randomNumber);
        combinedObject.transform.position = mesh1.transform.position;
        combinedObject.transform.rotation = mesh1.transform.rotation;
        combinedObject.transform.localScale = mesh1.transform.localScale;

        // Add MeshFilter and MeshRenderer
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Store world-to-local matrix of the combinedObject
        Matrix4x4 worldToLocal = combinedObject.transform.worldToLocalMatrix;

        // Setup CombineInstances
        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = mesh1.sharedMesh;
        combine[0].transform = worldToLocal * mesh1.transform.localToWorldMatrix;
        combine[1].mesh = mesh2.sharedMesh;
        combine[1].transform = worldToLocal * mesh2.transform.localToWorldMatrix;

        // Combine
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh" + randomNumber;
        combinedMesh.CombineMeshes(combine, true, true);
        combinedMeshFilter.mesh = combinedMesh;
        combinedMeshRenderer.material = mesh1.GetComponent<MeshRenderer>().sharedMaterial;

        // Add Rigidbody
        Rigidbody rb = combinedObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true;

        // Sync physics
        Physics.SyncTransforms();
        StartCoroutine(EnablePhysicsNextFrame(rb));

        // Disable original renderers
        mesh1.GetComponent<MeshRenderer>().enabled = false;
        mesh2.GetComponent<MeshRenderer>().enabled = false;

        // Copy XRGrabInteractable from mesh1
        XRGrabInteractable oldGrab = mesh1.GetComponent<XRGrabInteractable>();

        // Destroy old components
        Destroy(mesh1.GetComponent<XRGeneralGrabTransformer>());
        Destroy(mesh2.GetComponent<XRGeneralGrabTransformer>());
        Destroy(mesh1.GetComponent<XRGrabInteractable>());
        Destroy(mesh2.GetComponent<XRGrabInteractable>());
        Destroy(mesh1.GetComponent<Rigidbody>());
        Destroy(mesh2.GetComponent<Rigidbody>());

        // Create a new "Colliders" child object
        GameObject collidersContainer = new GameObject("Colliders");
        collidersContainer.transform.SetParent(combinedObject.transform, false);

        // Parent the original objects to "Colliders"
        mesh1.transform.SetParent(collidersContainer.transform, true);
        mesh2.transform.SetParent(collidersContainer.transform, true);

        // Re-apply XRGrabInteractable
        if (oldGrab != null)
        {
            XRGrabInteractable newGrab = combinedObject.AddComponent<XRGrabInteractable>();
            newGrab.interactionLayers = oldGrab.interactionLayers;
            newGrab.movementType = oldGrab.movementType;
            newGrab.trackPosition = oldGrab.trackPosition;
            newGrab.trackRotation = oldGrab.trackRotation;
            newGrab.smoothPosition = oldGrab.smoothPosition;
            newGrab.smoothRotation = oldGrab.smoothRotation;
            newGrab.throwOnDetach = oldGrab.throwOnDetach;
            newGrab.throwSmoothingDuration = oldGrab.throwSmoothingDuration;
            //newGrab.velocityDampen = oldGrab.velocityDampen;
            //newGrab.angularVelocityDampen = oldGrab.angularVelocityDampen;
            newGrab.attachTransform = oldGrab.attachTransform;
            newGrab.selectMode = oldGrab.selectMode;
            newGrab.useDynamicAttach = oldGrab.useDynamicAttach;
        }

        // Re-apply XRGeneralGrabTransformer if available
        XRGeneralGrabTransformer oldTransformer = mesh1.GetComponent<XRGeneralGrabTransformer>();
        if (oldTransformer != null)
        {
            combinedObject.AddComponent<XRGeneralGrabTransformer>();
        }

        Debug.Log("Meshes combined successfully with XR Grab support.");
    }

    IEnumerator EnablePhysicsNextFrame(Rigidbody rb)
    {
        yield return null; // Wait one frame
        rb.isKinematic = false; // Reactivate physics
        Destroy(this.gameObject);
    }
}
