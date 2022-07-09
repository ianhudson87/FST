// using System.Numerics;
// using System.Xml.Xsl.Runtime;
// using System.Security.Cryptography;
// using System.Numerics;
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
        other.gameObject.transform.position = other.gameObject.transform.position - difference * 0.95f; 
    }
}
