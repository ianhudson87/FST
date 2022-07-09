using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{

    [SerializeField] Portal p1, p2;

    public void teleport(Collider other, Portal source) {
        Vector3 difference;
        if (p1 == source) {
            difference =  p1.transform.position - p2.transform.position;
        }
        else {
            difference =  p2.transform.position - p1.transform.position;
        }
        Vector3 dest = other.gameObject.transform.position - difference * 0.95f;
        other.gameObject.GetComponent<Teleportable>()?.Teleport(dest);
    }
}
