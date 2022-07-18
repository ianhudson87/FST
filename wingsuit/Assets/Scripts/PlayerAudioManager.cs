using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AudioSource flyingAudioSrc, turbulanceAudioSrc;
    [SerializeField] float flyingAudioMinSpeed, flyingAudioMaxSpeed;
    [SerializeField] float turbulanceAudioMinNormalMagnitude, turbulanceAudioMaxNormalMagnitude;

    void Update() {
        FlyingAudio();
        TurbulanceAudio();
        SetMasterVolume();
    }

    void SetMasterVolume() {
        // Debug.Log(SettingsManager.Instance.masterVolume);
        AudioListener.volume = SettingsManager.Instance.masterVolume;
    }

    void FlyingAudio() {
        // all flying audio for players is handled on client. 
        float playerSpeed  = playerController.GetVelocity().magnitude;
        float flyingAudioVolume = Mathf.Clamp((playerSpeed - flyingAudioMinSpeed) / (flyingAudioMaxSpeed - flyingAudioMinSpeed), 0, 1);
        flyingAudioSrc.volume = flyingAudioVolume;
    }

    void TurbulanceAudio() {
        float gliderNormalForceMagnitude = playerController.GetGliderNormalForce().magnitude;
        // Debug.Log("normal mag" + gliderNormalForceMagnitude);
        float turbulanceAudioVolume = Mathf.Clamp((gliderNormalForceMagnitude - turbulanceAudioMinNormalMagnitude) / (turbulanceAudioMaxNormalMagnitude - turbulanceAudioMinNormalMagnitude), 0, 1);
        // Debug.Log("volu" + turbulanceAudioVolume);
        turbulanceAudioSrc.volume = turbulanceAudioVolume;
    }
}
