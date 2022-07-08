using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float keyboardSensitivity;
    [SerializeField] float sensitivity;
    PhotonView PV;

    // private Vector3 globalVelocity = new Vector3();

    private Rigidbody rb;

    private float verticalLookRotation;

    [SerializeField] private float airFrictionCoefficient;
    [SerializeField] private float airFrictionExponent;

    void Awake() {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        if(!PV.IsMine) {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            // Destroy(rb);
        }
    }

    void Update() {
        if(!PV.IsMine) {
            return;
        }

        Look();
        Glide();
    }

    void Look() {
        transform.Rotate(-Vector3.forward * Input.GetAxisRaw("Horizontal") * Time.deltaTime * keyboardSensitivity);

        transform.Rotate(Vector3.right * Input.GetAxisRaw("Vertical") * Time.deltaTime * keyboardSensitivity);
        // transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * Time.deltaTime * keyboardSensitivity / 1.5f);
    }

    void MouseLook() {
        // Yaw *not really because world space
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
        // transform.Rotate(Vector3.up,)
        // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);

        // Roll
        // get difference between desired and actual horizontal angle (angle along the xz-plane). Roll depending on that
        float desiredXZAngle = Mathf.Atan(transform.forward.z/(transform.forward.x + 1e-9f)) + ((transform.forward.x < 0) ? Mathf.PI : 0);
        float actualXZAngle = Mathf.Atan(rb.velocity.z/(rb.velocity.x + 1e-9f)) + ((rb.velocity.x < 0) ? Mathf.PI : 0);

        float deltaXZAngle = (desiredXZAngle % Mathf.PI) - (actualXZAngle % Mathf.PI);
        transform.Rotate(Vector3.forward * deltaXZAngle / Mathf.PI * 180 * Time.deltaTime);
        try{
            // if(transform.localEulerAngles.z < 90 && transform.localEulerAngles.z > -90)
            
        }
        catch {
            Debug.Log(deltaXZAngle);
            Debug.Log(actualXZAngle);
        }
        transform.Rotate(-Vector3.forward * transform.localEulerAngles.z/100);

        // Debug.Log(Mathf.Atan(transform.forward.z/transform.forward.x));
        // Debug.Log(desiredXZAngle);
        
        // Pitch *not really because world space
        transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * sensitivity);

        // verticalLookRotation += Input.GetAxisRaw("Mouse Y") * sensitivity;
        // verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
        // cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Glide() {
        // get the component of velocity in the direction perpendicular to the glider
        Vector3 gliderNormalForce = transform.up * Mathf.Pow(Mathf.Abs(Vector3.Dot(transform.up, rb.velocity)), airFrictionExponent) * airFrictionCoefficient * Mathf.Sign(Vector3.Dot(-transform.up, rb.velocity));

        // Debug.Log(gliderNormalForce + " : " + rb.velocity);

        Debug.DrawLine(transform.position, transform.position + gliderNormalForce, Color.blue, 0.1f);
        rb.AddForce(gliderNormalForce);

        if(Input.GetKey(KeyCode.Space)) {
            rb.AddForce(transform.forward);
        }
        // rb.velocity += gliderNormalForce * Time.deltaTime;
    }
}
