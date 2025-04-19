using System.Collections;
using UnityEngine;

public class GlueScript : MonoBehaviour
{
    [SerializeField]
    GameObject firstPiece;

    [SerializeField]
    GameObject secondPiece;
    bool stopGluing;

    private bool preserveScale = true;
    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Combine();
            Combine();
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

                Debug.Log("2 PIECES");
                stopGluing = true;

                StartCoroutine(CombineAfterDelay(3f));
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
        Debug.Log($"Paused gluing for {delay} seconds...");

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Combine the meshes after the delay
        stopGluing = false;
    }

    IEnumerator CombineAfterDelay(float delay)
    {
        Debug.Log($"Combining meshes in {delay} seconds...");

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Combine the meshes after the delay
        //Combine();
        CombineMeshes();
    }

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
        rb.useGravity = true;           // Enable gravity
        rb.isKinematic = true;         // Let physics affect it

        // Sync physics (optional but good)
        Physics.SyncTransforms();

        // Enable Rigidbody physics after one frame
        StartCoroutine(EnablePhysicsNextFrame(rb));

        // Add a MeshCollider (optional but recommended for physics)
        //MeshCollider collider = combinedObject.AddComponent<MeshCollider>();
        //collider.sharedMesh = combinedMesh;
        //collider.convex = true;         // Required for Rigidbody interaction

        // Disable original renderers
        mesh1.GetComponent<MeshRenderer>().enabled = false;
        mesh2.GetComponent<MeshRenderer>().enabled = false;

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

    //void CombineMeshes()
    //{
    //    MeshFilter firstPiece = firstPiece.GetComponent<MeshFilter>();
    //    MeshFilter secondPiece = secondPiece.GetComponent<MeshFilter>();

    //    // Combine the meshes
    //    CombineInstance[] combine = new CombineInstance[2];

    //    combine[0].mesh = firstPiece.sharedMesh;
    //    combine[0].transform = firstPiece.transform.localToWorldMatrix;

    //    combine[1].mesh = secondPiece.sharedMesh;
    //    combine[1].transform = secondPiece.transform.localToWorldMatrix;

    //    // Create a new mesh and combine
    //    Mesh combinedMesh = new Mesh();
    //    combinedMesh.CombineMeshes(combine, true, true);

    //    // Calculate the center of the combined mesh
    //    Bounds bounds = combinedMesh.bounds;
    //    Vector3 center = bounds.center;

    //    // Move the vertices back so the mesh stays in the correct world position
    //    Vector3[] vertices = combinedMesh.vertices;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        vertices[i] -= center;
    //    }
    //    combinedMesh.vertices = vertices;
    //    combinedMesh.RecalculateBounds();

    //    // Assign the combined mesh to firstPiece
    //    firstPiece.mesh = combinedMesh;

    //    // Adjust the position of firstPiece to center the pivot
    //    firstPiece.transform.position += center;

    //    // Disable secondPiece
    //    secondPiece.gameObject.SetActive(false);

    //    Debug.Log("Meshes combined successfully! Combined mesh assigned to firstPiece with centered pivot.");

    //    //if (firstPiece == null || secondPiece == null)
    //    //{
    //    //    Debug.LogError("Please assign both meshes in the Inspector.");
    //    //    return;
    //    //}

    //    //// Create a new GameObject to hold the combined mesh
    //    //GameObject combinedObject = new GameObject("CombinedMesh");
    //    //combinedObject.transform.position = transform.position;
    //    //combinedObject.transform.rotation = transform.rotation;

    //    //// Add required components
    //    //MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
    //    //MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

    //    //// Combine the meshes
    //    //CombineInstance[] combine = new CombineInstance[2];

    //    //combine[0].mesh = firstPiece.sharedMesh;
    //    //combine[0].transform = firstPiece.transform.localToWorldMatrix;

    //    //combine[1].mesh = secondPiece.sharedMesh;
    //    //combine[1].transform = secondPiece.transform.localToWorldMatrix;

    //    //// Create a new mesh and combine
    //    //Mesh combinedMesh = new Mesh();
    //    //combinedMesh.CombineMeshes(combine, true, true);

    //    //// Assign the combined mesh to the new GameObject
    //    //combinedMeshFilter.mesh = combinedMesh;

    //    //// Calculate the center of the combined mesh
    //    //Bounds bounds = combinedMesh.bounds;
    //    //Vector3 center = bounds.center;

    //    //// Adjust the position of the combined mesh to center the pivot
    //    //combinedObject.transform.position += center;

    //    //// Move the vertices back so the mesh stays in the correct world position
    //    //Vector3[] vertices = combinedMesh.vertices;
    //    //for (int i = 0; i < vertices.Length; i++)
    //    //{
    //    //    vertices[i] -= center;
    //    //}
    //    //combinedMesh.vertices = vertices;
    //    //combinedMesh.RecalculateBounds();

    //    //// Assign a material (you can customize this)
    //    //combinedMeshRenderer.material = firstPiece.GetComponent<MeshRenderer>().sharedMaterial;

    //    //// Disable the original meshes (optional)
    //    //firstPiece.gameObject.SetActive(false);
    //    //secondPiece.gameObject.SetActive(false);

    //    //Debug.Log("Meshes combined successfully! Pivot centered.");
    //}

    void Combine()
    {
        // Check if both GameObjects are assigned
        if (firstPiece == null || secondPiece == null)
        {
            Debug.LogError("Please assign both GameObjects.");
            return;
        }

        // Get the MeshFilter and MeshRenderer components
        MeshFilter meshFilter1 = firstPiece.GetComponent<MeshFilter>();
        MeshFilter meshFilter2 = secondPiece.GetComponent<MeshFilter>();

        if (meshFilter1 == null || meshFilter2 == null)
        {
            Debug.LogError("One or both objects are missing MeshFilter.");
            return;
        }

        // Create combine instances for both meshes
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = meshFilter1.sharedMesh;
        combine[0].transform = meshFilter1.transform.localToWorldMatrix;

        combine[1].mesh = meshFilter2.sharedMesh;
        combine[1].transform = meshFilter2.transform.localToWorldMatrix;

        // Create the new combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true); // merge into one submesh, use world matrices

        // Assign combined mesh to the first piece
        meshFilter1.mesh = combinedMesh;

        // Optional: Recalculate bounds/normals
        meshFilter1.mesh.RecalculateBounds();
        meshFilter1.mesh.RecalculateNormals();

        // Disable the second piece
        secondPiece.SetActive(false);

        Debug.Log("Meshes combined successfully into firstPiece.");
    }

}
