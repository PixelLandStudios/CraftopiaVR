using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public MeshFilter mesh1; // Assign the first mesh in the Inspector
    public MeshFilter mesh2; // Assign the second mesh in the Inspector

    private bool combined = false; // To track if meshes are already combined

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !combined)
        {
            CombineMeshes();
            combined = true; // Prevent combining again
        }
    }

    void CombineMeshes()
    {
        if (mesh1 == null || mesh2 == null)
        {
            Debug.LogError("Please assign both meshes in the Inspector.");
            return;
        }

        // Create a new mesh to hold the combined result
        Mesh combinedMesh = new Mesh();

        // Combine the meshes in local space
        CombineInstance[] combine = new CombineInstance[2];

        // Use the localToWorldMatrix to account for position, rotation, and scale
        combine[0].mesh = mesh1.sharedMesh;
        combine[0].transform = mesh1.transform.localToWorldMatrix;

        combine[1].mesh = mesh2.sharedMesh;
        combine[1].transform = mesh2.transform.localToWorldMatrix;

        // Combine the meshes into the new mesh
        combinedMesh.CombineMeshes(combine, true, true);

        // Assign the combined mesh to mesh1
        mesh1.mesh = combinedMesh;

        // Replace the current collider of mesh1 with a convex mesh collider
        ReplaceColliderWithConvex(mesh1.gameObject, combinedMesh);

        // Disable mesh2
        mesh2.gameObject.SetActive(false);

        Debug.Log("Meshes combined successfully! Combined mesh assigned to mesh1.");
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