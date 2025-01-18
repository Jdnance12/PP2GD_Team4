using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;
    public Camera_Controller cameraController;

    private void Start()
    {
        // Set the slider's value to the current sensitivity setting, scaled down by 100
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 200.0f) / 100;
        UpdateSensitivityText(sensitivitySlider.value);

        // Add a listener to call the OnSensitivityChanged method whenever the slider's value changes
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        // Update the sensitivity setting, scaled up by 100
        PlayerPrefs.SetFloat("MouseSensitivity", value * 100);
        UpdateSensitivityText(value);

        // Update the camera controller's sensitivity
        if (cameraController != null)
        {
            cameraController.SetSensitivity(value);
        }
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("F0");
        }
    }
}
