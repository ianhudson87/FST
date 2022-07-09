using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam; // the one camera in our scene

    void Update()
    {
        if(cam == null) {
            cam = FindObjectOfType<Camera>();
        }

        if(cam == null) {
            // didn't find the camera
            return;
        }

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up, 180f);
    }
}
