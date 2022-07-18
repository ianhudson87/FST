using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] TextMeshProUGUI mouseSensitivityValueText;
    [SerializeField] TextMeshProUGUI controllerSensitivityValueText;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider controllerSensitivitySlider;
    [HideInInspector] public float mouseSensitivity;
    [HideInInspector] public float controllerSensitivity;
    [SerializeField] Slider volumeSlider;
    [HideInInspector] public float masterVolume;

    void Awake() {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        if(PlayerPrefs.HasKey("MouseSensitivity")) {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        if(PlayerPrefs.HasKey("ControllerSensitivity")) {
            controllerSensitivitySlider.value = PlayerPrefs.GetFloat("ControllerSensitivity");
        }
        OnSensitivitySliderChange();
        OnVolumeSliderChange();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            settingsCanvas.SetActive(!settingsCanvas.activeSelf);
        }
    }

    public void OnSensitivitySliderChange() {
        mouseSensitivity = mouseSensitivitySlider.value;
        mouseSensitivityValueText.text = mouseSensitivity.ToString();
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        controllerSensitivity = controllerSensitivitySlider.value;
        controllerSensitivityValueText.text = controllerSensitivity.ToString();
        PlayerPrefs.SetFloat("ControllerSensitivity", controllerSensitivity);
    }

    public void OnVolumeSliderChange() {
        masterVolume = volumeSlider.value;
    }
}
