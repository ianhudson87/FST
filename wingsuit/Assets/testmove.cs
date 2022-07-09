using System.Threading;
// using System.Numerics;
// using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0) {
            transform.position += Vector3.right * Time.deltaTime * 5;
        }
        if (Input.GetAxisRaw("Horizontal") < 0 ) {
            transform.position += -Vector3.right * Time.deltaTime * 5;
        }

    }
}
