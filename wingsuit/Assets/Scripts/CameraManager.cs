using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float camMinSpeed, camMaxSpeed, camDistMultiplier;
    [SerializeField] PlayerController pc;
    Vector3 cameraDefaultLocalPos;
    Quaternion cameraDefaultLocalRot;
    [SerializeField] Transform rearCamTransform;
    bool rearView = false;

    void Awake() {
        cameraDefaultLocalPos = transform.localPosition;
        cameraDefaultLocalRot = transform.localRotation;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyUp(KeyCode.F)) {
            Debug.Log("active");
            rearView = !rearView;
        }

        if(rearView) {
            // rearViewCam
            this.transform.position = rearCamTransform.position;
            this.transform.rotation = rearCamTransform.rotation;
        }
        else {
            //
            float speedPercentage = Mathf.Clamp((pc.GetVelocity().magnitude - camMinSpeed) / (camMaxSpeed-camMinSpeed), 0, 1);
            transform.localPosition = cameraDefaultLocalPos * (speedPercentage * (camDistMultiplier - 1) + 1);
            transform.localRotation = cameraDefaultLocalRot;
        }
        
    }

}
