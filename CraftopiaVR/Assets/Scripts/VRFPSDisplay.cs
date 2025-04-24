using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class VRFPSDisplay : MonoBehaviour
{
    [SerializeField]
    GameObject FPS;

    public TextMeshProUGUI fpsText;

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";

        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool aButtonPressed) && aButtonPressed)
        {
            FPS.SetActive(!FPS.activeSelf);
        }
    }
}