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

            GameObject lowerPart = slicedHull.CreateLowerHull(woodPlankToCut);
            GameObject upperPart = slicedHull.CreateUpperHull(woodPlankToCut);

            woodPlankToCut.SetActive(false);

            SetupSlicedWoodPlank(lowerPart, woodPlankToCut);
            SetupSlicedWoodPlank(upperPart, woodPlankToCut);

            woodPlankToCut = null;
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

        XRGeneralGrabTransformer xrGeneralGrabTransformer = slicedWoodPlank.AddComponent<XRGeneralGrabTransformer>();

        //foreach (Component component in sourceGameObject.GetComponents<Component>())
        //{
        //    // Skip the Transform component, as it is required and already exists
        //    if (component is Transform || component is MeshFilter || component is MeshRenderer) continue;

        //    // Check if the component already exists on the new game object
        //    Component existingComponent = slideWoodPlank.GetComponent(component.GetType());

        //    if (existingComponent != null)
        //    {
        //        // If the component already exists, copy its properties
        //        foreach (var field in component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
        //        {
        //            field.SetValue(existingComponent, field.GetValue(component));
        //        }

        //        foreach (var prop in component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
        //        {
        //            if (prop.CanWrite && prop.CanRead)
        //            {
        //                prop.SetValue(existingComponent, prop.GetValue(component));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // If the component does not exist, add it and copy its properties
        //        Component copiedComponent = slideWoodPlank.AddComponent(component.GetType());

        //        foreach (var field in component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
        //        {
        //            field.SetValue(copiedComponent, field.GetValue(component));
        //        }

        //        foreach (var prop in component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
        //        {
        //            if (prop.CanWrite && prop.CanRead)
        //            {
        //                prop.SetValue(copiedComponent, prop.GetValue(component));
        //            }
        //        }
        //    }
        //}
    }
}