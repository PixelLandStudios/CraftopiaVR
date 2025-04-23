using EzySlice;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

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
        Debug.Log("ENTERED: " + other.name);

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
            Material woodPlankMaterial = woodPlankToCut.GetComponent<MeshRenderer>().sharedMaterial;
            SlicedHull slicedHull = woodPlankToCut.Slice(CuttingPlaneTransform.position, CuttingPlaneTransform.up, woodPlankMaterial);

            if (slicedHull == null)
                return;

            GameObject lowerPart = slicedHull.CreateLowerHull(woodPlankToCut);
            GameObject upperPart = slicedHull.CreateUpperHull(woodPlankToCut);

            woodPlankToCut.SetActive(false);

            SetupSlicedWoodPlank(lowerPart, woodPlankToCut);
            SetupSlicedWoodPlank(upperPart, woodPlankToCut);

            woodPlankToCut = null;

            Debug.Log("CUTTT");
        }
    }

    void SetupSlicedWoodPlank(GameObject slicedWoodPlank, GameObject originalWoodPlank)
    {
        slicedWoodPlank.tag = "WoodPlank";

        Rigidbody rigidbody = slicedWoodPlank.AddComponent<Rigidbody>();
        rigidbody.mass = originalWoodPlank.GetComponent<Rigidbody>().mass / 2;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        MeshCollider collider = slicedWoodPlank.AddComponent<MeshCollider>();
        collider.convex = true;

        XRGrabInteractable xrGrabInteractable = slicedWoodPlank.AddComponent<XRGrabInteractable>();
        xrGrabInteractable.distanceCalculationMode = XRBaseInteractable.DistanceCalculationMode.ColliderPosition;
        xrGrabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        xrGrabInteractable.useDynamicAttach = true;
        xrGrabInteractable.throwOnDetach = false;

        XRGeneralGrabTransformer xrGeneralGrabTransformer = slicedWoodPlank.AddComponent<XRGeneralGrabTransformer>();
    }
}