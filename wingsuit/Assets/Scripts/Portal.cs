using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        GetComponentInParent<PortalManager>().teleport(other);
    }
}
