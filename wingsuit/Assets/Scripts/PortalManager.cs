using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PortalManager : MonoBehaviour
{

    [SerializeField] Portal p1, p2;
    [SerializeField] Camera c1, c2;
    Material p1Material, p2Material;

    Camera localPlayerCamera = null;

    void Start() {
        // create material for portal 1
        // create render texture for portal 1
        p1Material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        p1.gameObject.GetComponent<MeshRenderer>().material = p1Material;
        Debug.Log("SHADER" + Shader.Find("Unlit/ScreenCutoutShader"));
        c2.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        p1Material.mainTexture = c2.targetTexture;

        p2Material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        p2.gameObject.GetComponent<MeshRenderer>().material = p2Material;
        // Debug.Log("SHADER" + Shader.Find("ScreenCutoutShader"));
        c1.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        p2Material.mainTexture = c1.targetTexture;
    }

    void Update() {
        if(localPlayerCamera == null) {
            PlayerController[] playerControllers = GameObject.FindObjectsOfType<PlayerController>();
            // Debug.Log("Player Controolller" + playerControllers);
            foreach (PlayerController pc in playerControllers) {
                if(pc.PV.IsMine) {
                    localPlayerCamera = pc.GetComponentInChildren<Camera>();
                    break;
                }
            }
        }
        else {
            Vector3 cam2DeltaPos = p1.transform.position - localPlayerCamera.gameObject.transform.position;
            c2.transform.position = p2.transform.position - cam2DeltaPos;
            c2.transform.rotation = localPlayerCamera.gameObject.transform.rotation;
            // c2.nearClipPlane = cam2DeltaPos.magnitude;
            // c2.farClipPlane = 1000;


            Vector3 cam1DeltaPos = p2.transform.position - localPlayerCamera.gameObject.transform.position;
            c1.transform.position = p1.transform.position - cam1DeltaPos;
            c1.transform.rotation = localPlayerCamera.gameObject.transform.rotation;
            // c1.nearClipPlane = cam1DeltaPos.magnitude;
            // c1.farClipPlane = 1000;
        }        
        
    }

    public void teleport(Collider other, Portal source) {
        Vector3 difference;
        if (p1 == source) {
            difference =  p1.transform.position - p2.transform.position;
        }
        else {
            difference =  p2.transform.position - p1.transform.position;
        }
        Debug.Log("attempt instant transmission!");
        Vector3 dest = other.gameObject.transform.position - difference * 0.9f;
        Debug.Log(other.gameObject.GetComponent<Teleportable>());
        other.gameObject.GetComponentInParent<Teleportable>()?.Teleport(dest);
    }
}
