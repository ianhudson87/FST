using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AudioSource flyingAudioSrc;
    [SerializeField] float flyingAudioMinSpeed, flyingAudioMaxSpeed;

    void Update() {
        FlyingAudio();
    }

    void FlyingAudio() {
        // all flying audio for players is handled on client. 
        float playerSpeed  = playerController.GetSpeed();
        float flyingAudioVolume = Mathf.Clamp((playerSpeed - flyingAudioMinSpeed) / (flyingAudioMaxSpeed - flyingAudioMinSpeed), 0, 1);
        flyingAudioSrc.volume = flyingAudioVolume;
    }
}
