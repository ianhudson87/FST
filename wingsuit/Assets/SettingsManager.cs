using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviourPunCallbacks
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
    [SerializeField] Button LeaveRoomButton;

    void Awake() {
        if(Instance != null) {
            Destroy(this.gameObject);
            return;
        }

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

    public override void OnJoinedRoom()
    {
        // Debug.Log("enable button");
        base.OnJoinedRoom();
        LeaveRoomButton.gameObject.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        LeaveRoomButton.gameObject.SetActive(false);
    }
}
