using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float sensitivity;

    // private Vector3 globalVelocity = new Vector3();

    private Rigidbody rb;

    private float verticalLookRotation;

    [SerializeField] private float airFrictionCoefficient;
    [SerializeField] private float airFrictionExponent;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        Look();
        Glide();
    }

    void Look() {
        // Yaw
        transform.Rotate(Vector3.back * Input.GetAxisRaw("Mouse X") * sensitivity);

        // Roll
        // get difference between desired and actual horizontal angle (angle along the xz-plane). Roll depending on that
        // desiredXZAngle = transform.up
        // actualXZAngle = 
        
        // Pitch
        transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * sensitivity);

        // verticalLookRotation += Input.GetAxisRaw("Mouse Y") * sensitivity;
        // verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
        // cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Glide() {
        // get the component of velocity in the direction perpendicular to the glider
        Vector3 gliderNormalForce = transform.forward * Mathf.Pow(Mathf.Abs(Vector3.Dot(transform.forward, rb.velocity)), airFrictionExponent) * airFrictionCoefficient * Mathf.Sign(Vector3.Dot(-transform.forward, rb.velocity));

        // Debug.Log(gliderNormalForce + " : " + rb.velocity);

        Debug.DrawLine(transform.position, transform.position + gliderNormalForce, Color.blue, 0.1f);
        rb.AddForce(gliderNormalForce);
        // rb.velocity += gliderNormalForce * Time.deltaTime;
    }
}
