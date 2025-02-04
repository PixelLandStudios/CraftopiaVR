using EzySlice;
using System.Collections.Generic;
using UnityEngine;

public class MiterSawBladeScript : MonoBehaviour
{
    [SerializeField]
    Transform CuttingPlaneTransform;

    GameObject woodPlankToCut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WoodPlank"))
        {
            Debug.Log("Added");
            woodPlankToCut = other.gameObject;
        }
        else if (other.CompareTag("MiterSawBase"))
        {
            if (woodPlankToCut == null)
                return;

            //cut the wood plank
            SlicedHull slicedHull = woodPlankToCut.Slice(CuttingPlaneTransform.position, CuttingPlaneTransform.up);

            slicedHull.CreateLowerHull(woodPlankToCut);
            slicedHull.CreateUpperHull(woodPlankToCut);

            woodPlankToCut.SetActive(false);

            woodPlankToCut = null;
        }
    }
}
