using System.Collections;
using UnityEngine;

public class GlueScript : MonoBehaviour
{
    [SerializeField]
    GameObject firstPiece;

    [SerializeField]
    GameObject secondPiece;
    bool stopGluing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Combine();
            CombineMeshes();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stopGluing)
            return;

        if (other.CompareTag("WoodPlank"))
        {
            Debug.Log(other.name);

            if (firstPiece == null)
                firstPiece = other.gameObject;
            else if (secondPiece == null)
            {
                secondPiece = other.gameObject;

                Debug.Log("2 PIECES");
                stopGluing = true;

                StartCoroutine(CombineAfterDelay(3f));
            }
        }
    }

    IEnumerator CombineAfterDelay(float delay)
    {
        Debug.Log($"Combining meshes in {delay} seconds...");

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Combine the meshes after the delay
        Combine();
    }

    void CombineMeshes()
    {
        MeshFilter mesh1 = firstPiece.GetComponent<MeshFilter>();
        MeshFilter mesh2 = secondPiece.GetComponent<MeshFilter>();

        // Combine the meshes
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = mesh1.sharedMesh;
        combine[0].transform = mesh1.transform.localToWorldMatrix;

        combine[1].mesh = mesh2.sharedMesh;
        combine[1].transform = mesh2.transform.localToWorldMatrix;

        // Create a new mesh and combine
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        // Calculate the center of the combined mesh
        Bounds bounds = combinedMesh.bounds;
        Vector3 center = bounds.center;

        // Move the vertices back so the mesh stays in the correct world position
        Vector3[] vertices = combinedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= center;
        }
        combinedMesh.vertices = vertices;
        combinedMesh.RecalculateBounds();

        // Assign the combined mesh to mesh1
        mesh1.mesh = combinedMesh;

        // Adjust the position of mesh1 to center the pivot
        mesh1.transform.position += center;

        // Disable mesh2
        mesh2.gameObject.SetActive(false);

        Debug.Log("Meshes combined successfully! Combined mesh assigned to mesh1 with centered pivot.");

        //if (mesh1 == null || mesh2 == null)
        //{
        //    Debug.LogError("Please assign both meshes in the Inspector.");
        //    return;
        //}

        //// Create a new GameObject to hold the combined mesh
        //GameObject combinedObject = new GameObject("CombinedMesh");
        //combinedObject.transform.position = transform.position;
        //combinedObject.transform.rotation = transform.rotation;

        //// Add required components
        //MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        //MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

        //// Combine the meshes
        //CombineInstance[] combine = new CombineInstance[2];

        //combine[0].mesh = mesh1.sharedMesh;
        //combine[0].transform = mesh1.transform.localToWorldMatrix;

        //combine[1].mesh = mesh2.sharedMesh;
        //combine[1].transform = mesh2.transform.localToWorldMatrix;

        //// Create a new mesh and combine
        //Mesh combinedMesh = new Mesh();
        //combinedMesh.CombineMeshes(combine, true, true);

        //// Assign the combined mesh to the new GameObject
        //combinedMeshFilter.mesh = combinedMesh;

        //// Calculate the center of the combined mesh
        //Bounds bounds = combinedMesh.bounds;
        //Vector3 center = bounds.center;

        //// Adjust the position of the combined mesh to center the pivot
        //combinedObject.transform.position += center;

        //// Move the vertices back so the mesh stays in the correct world position
        //Vector3[] vertices = combinedMesh.vertices;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i] -= center;
        //}
        //combinedMesh.vertices = vertices;
        //combinedMesh.RecalculateBounds();

        //// Assign a material (you can customize this)
        //combinedMeshRenderer.material = mesh1.GetComponent<MeshRenderer>().sharedMaterial;

        //// Disable the original meshes (optional)
        //mesh1.gameObject.SetActive(false);
        //mesh2.gameObject.SetActive(false);

        //Debug.Log("Meshes combined successfully! Pivot centered.");
    }

    void Combine()
    {
        // Check if both GameObjects are assigned
        if (firstPiece == null || secondPiece == null)
        {
            Debug.LogError("Please assign both GameObjects in the Inspector.");
            return;
        }

        // Get the MeshFilter components of both objects
        MeshFilter meshFilter1 = firstPiece.GetComponent<MeshFilter>();
        MeshFilter meshFilter2 = secondPiece.GetComponent<MeshFilter>();

        if (meshFilter1 == null || meshFilter2 == null)
        {
            Debug.LogError("One or both GameObjects do not have a MeshFilter component.");
            return;
        }

        // Create an array to hold the combine instances
        CombineInstance[] combine = new CombineInstance[2];

        // Set the mesh and transform for the first object
        combine[0].mesh = meshFilter1.sharedMesh;
        combine[0].transform = meshFilter1.transform.localToWorldMatrix;

        // Set the mesh and transform for the second object
        combine[1].mesh = meshFilter2.sharedMesh;
        combine[1].transform = meshFilter2.transform.localToWorldMatrix;

        // Create a new mesh and combine the meshes into it
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // Assign the combined mesh to the current GameObject
        MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
        currentMeshFilter.mesh = combinedMesh;

        // Optionally, enable the MeshRenderer if it was disabled
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        // Disable the second GameObject after combining
        //secondPiece.SetActive(false);

        Debug.Log("Meshes combined successfully!");
    }
}
