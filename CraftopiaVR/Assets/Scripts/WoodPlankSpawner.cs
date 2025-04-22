using UnityEngine;

public class WoodPlankSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject WoodPlankPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WoodPlank"))
        {
            Debug.Log("Exited");

            Instantiate(WoodPlankPrefab, this.transform.position, Quaternion.identity);
        }
    }
}
