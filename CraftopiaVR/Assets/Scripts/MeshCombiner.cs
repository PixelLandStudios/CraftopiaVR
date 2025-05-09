using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class MeshCombiner : MonoBehaviour
{
    [SerializeField]
    GameObject firstPiece;

    [SerializeField]
    GameObject secondPiece;

    private bool combined = false; // To track if meshes are already combined

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !combined)
        {
            CombineMeshes();
            combined = true; // Prevent combining again
        }
    }

    //void CombineMeshes()
    //{
    //    if (mesh1 == null || mesh2 == null)
    //    {
    //        Debug.LogError("Please assign both meshes in the Inspector.");
    //        return;
    //    }

    //    // Create a new mesh to hold the combined result
    //    Mesh combinedMesh = new Mesh();

    //    // Combine the meshes in local space
    //    CombineInstance[] combine = new CombineInstance[2];

    //    // Use the localToWorldMatrix to account for position, rotation, and scale
    //    combine[0].mesh = mesh1.sharedMesh;
    //    combine[0].transform = mesh1.transform.localToWorldMatrix;

    //    combine[1].mesh = mesh2.sharedMesh;
    //    combine[1].transform = mesh2.transform.localToWorldMatrix;

    //    // Combine the meshes into the new mesh
    //    combinedMesh.CombineMeshes(combine, true, true);

    //    // Assign the combined mesh to mesh1
    //    mesh1.mesh = combinedMesh;

    //    // Replace the current collider of mesh1 with a convex mesh collider
    //    ReplaceColliderWithConvex(mesh1.gameObject, combinedMesh);

    //    // Disable mesh2
    //    mesh2.gameObject.SetActive(false);

    //    Debug.Log("Meshes combined successfully! Combined mesh assigned to mesh1.");
    //}

    void CombineMeshes()
    {
        MeshFilter mesh1 = firstPiece.GetComponent<MeshFilter>();
        MeshFilter mesh2 = secondPiece.GetComponent<MeshFilter>();

        if (mesh1 == null || mesh2 == null)
        {
            Debug.LogError("Please assign both meshes in the Inspector.");
            return;
        }

        // Create a new GameObject to hold the combined mesh at mesh1's position and rotation
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.position = mesh1.transform.position;
        combinedObject.transform.rotation = mesh1.transform.rotation;
        combinedObject.transform.localScale = mesh1.transform.localScale;

        // Add MeshFilter and MeshRenderer
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Store world-to-local matrix of the combinedObject to keep the mesh aligned
        Matrix4x4 worldToLocal = combinedObject.transform.worldToLocalMatrix;

        // Setup CombineInstances with transforms relative to the combinedObject
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = mesh1.sharedMesh;
        combine[0].transform = worldToLocal * mesh1.transform.localToWorldMatrix;

        combine[1].mesh = mesh2.sharedMesh;
        combine[1].transform = worldToLocal * mesh2.transform.localToWorldMatrix;

        // Combine the meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";
        combinedMesh.CombineMeshes(combine, true, true);

        // Assign combined mesh
        combinedMeshFilter.mesh = combinedMesh;

        // Use the first mesh's material (customize if needed)
        combinedMeshRenderer.material = mesh1.GetComponent<MeshRenderer>().sharedMaterial;

        // Add Rigidbody
        Rigidbody rb = combinedObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true; // Disable until next frame

        // Sync physics transforms
        Physics.SyncTransforms();

        // Enable Rigidbody physics after one frame
        StartCoroutine(EnablePhysicsNextFrame(rb));

        // Disable original mesh renderers
        mesh1.GetComponent<MeshRenderer>().enabled = false;
        mesh2.GetComponent<MeshRenderer>().enabled = false;

        Destroy(mesh1.GetComponent<XRGeneralGrabTransformer>());
        Destroy(mesh2.GetComponent<XRGeneralGrabTransformer>());

        Destroy(mesh1.GetComponent<XRGrabInteractable>());
        Destroy(mesh2.GetComponent<XRGrabInteractable>());

        Destroy(mesh1.GetComponent<Rigidbody>());
        Destroy(mesh2.GetComponent<Rigidbody>());

        // Optionally parent the originals to the combined mesh
        mesh1.transform.SetParent(combinedObject.transform, true);
        mesh2.transform.SetParent(combinedObject.transform, true);

        Debug.Log("Meshes combined successfully with Rigidbody and Collider.");
    }

    IEnumerator EnablePhysicsNextFrame(Rigidbody rb)
    {
        yield return null; // Wait one frame
        rb.isKinematic = false; // Reactivate physics
    }

    void ReplaceColliderWithConvex(GameObject target, Mesh mesh)
    {
        // Remove existing colliders
        Collider[] existingColliders = target.GetComponents<Collider>();
        foreach (Collider collider in existingColliders)
        {
            Destroy(collider);
        }

        // Add a MeshCollider and set it to convex
        MeshCollider meshCollider = target.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}