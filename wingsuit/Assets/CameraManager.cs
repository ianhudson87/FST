using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float camMinSpeed, camMaxSpeed, camDistMultiplier;
    [SerializeField] PlayerController pc;
    Vector3 cameraDefaultPos;

    void Awake() {
        cameraDefaultPos = transform.localPosition;
    }

    void Update() {
        float speedPercentage = Mathf.Clamp((pc.GetVelocity().magnitude - camMinSpeed) / (camMaxSpeed-camMinSpeed), 0, 1);
        transform.localPosition = cameraDefaultPos * (speedPercentage * (camDistMultiplier - 1) + 1);
    }

}
