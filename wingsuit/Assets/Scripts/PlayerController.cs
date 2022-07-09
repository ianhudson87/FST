using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, Teleportable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float controllerSensitivity;
    [SerializeField] float sensitivity;
    [SerializeField] float gravity;
    [SerializeField] GameObject model;
    [SerializeField] float maxRollAngle;
    TrailRenderer trail;
    PhotonView PV;
    PlayerManager playerManager;
    Vector3 gravityVector;
    

    // private Vector3 globalVelocity = new Vector3();

    private Rigidbody rb;

    private float verticalLookRotation;

    [SerializeField] private float airFrictionCoefficient;
    [SerializeField] private float airFrictionExponent;

    void Awake() {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        trail = GetComponent<TrailRenderer>();
        gravityVector = new Vector3(0, -gravity, 0);
    }

    void Start() {
        if(!PV.IsMine) {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            // Destroy(rb);
        }
    }

    void Update() {
        // if(transform.position.y < 5) {
        //     rb.isKinematic = true;
        //     transform.position += new Vector3(0, 50, 0);
        //     rb.isKinematic = false;
        // }

        if(!PV.IsMine) {
            return;
        }

        MouseLook();
        ControllerLook();
        Roll();
        Glide();
        Gravity();
    }

    void Gravity() {
        rb.AddForce(gravityVector);
    }

    void Look() {
        transform.Rotate(-Vector3.forward * Input.GetAxisRaw("Horizontal") * Time.deltaTime * controllerSensitivity);

        transform.Rotate(Vector3.right * Input.GetAxisRaw("Vertical") * Time.deltaTime * controllerSensitivity);
        // transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * Time.deltaTime * keyboardSensitivity / 1.5f);
    }

    // void MouseLookClickToRoll() {
    //     // Yaw *not really because world space
    //     transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
    //     // transform.Rotate(Vector3.up,)
    //     // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);

    //     // Roll. Only roll the model
    //     if(Input.GetMouseButton(0)) {
    //         model.transform.localEulerAngles += new Vector3(0, 0, 45 * Time.deltaTime);
    //     }
    //     if(Input.GetMouseButton(1)) {
    //         model.transform.localEulerAngles += new Vector3(0, 0, -45 * Time.deltaTime);
    //     }

    //     // else if(Input.GetAxisRaw("Mouse X") > 0) {
    //     //     model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, model.transform.localEulerAngles.y, -80);
    //     // }
    //     // else {
    //     //     model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, model.transform.localEulerAngles.y, 0);
    //     // }
        
    //     // Pitch *not really because world space
    //     transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * sensitivity);
    // }

    // void MouseLookSuperSharpTurns() {
    //     // Yaw *not really because world space
    //     transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
    //     // transform.Rotate(Vector3.up,)
    //     // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);

    //     // Roll. Only roll the model
    //     if(Input.GetAxisRaw("Mouse X") < 0) {
    //         model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, model.transform.localEulerAngles.y, 80);
    //     }
    //     else if(Input.GetAxisRaw("Mouse X") > 0) {
    //         model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, model.transform.localEulerAngles.y, -80);
    //     }
    //     else {
    //         model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, model.transform.localEulerAngles.y, 0);
    //     }
        
    //     // Pitch *not really because world space
    //     transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * sensitivity);
    // }
    void MouseLook() {
        // Yaw *not really because world space
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
        // transform.Rotate(Vector3.up,)
        // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);
        
        // Pitch *not really because world space
        transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * sensitivity);
    }

    void ControllerLook() {
        // Yaw *not really because world space
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * controllerSensitivity, Space.World);
        // transform.Rotate(Vector3.up,)
        // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);
        
        // Pitch *not really because world space
        transform.Rotate(Vector3.right * Input.GetAxisRaw("Vertical") * controllerSensitivity);
    }

    void Roll() {
        // Roll. Only roll the model
        // get difference between desired and actual horizontal angle (angle along the xz-plane). Roll depending on that
        float desiredXZAngle = Mathf.Atan(transform.forward.z/(transform.forward.x + 1e-9f)) + ((transform.forward.x < 0) ? Mathf.PI : 0);
        float actualXZAngle = Mathf.Atan(rb.velocity.z/(rb.velocity.x + 1e-9f)) + ((rb.velocity.x < 0) ? Mathf.PI : 0);

        float deltaXZAngle = (desiredXZAngle - actualXZAngle) / Mathf.PI * 180;
        while(deltaXZAngle > 360f) {
            deltaXZAngle -= 360f;
        }
        while(deltaXZAngle < 0f) {
            deltaXZAngle += 360f;
        }

        if (((deltaXZAngle > 5  && deltaXZAngle < 180 - 5) || (deltaXZAngle > 180 + 5  && deltaXZAngle < 360 - 5))  && rb.velocity.magnitude > 5) {
            // rotate to make the turn
            if(model.transform.localEulerAngles.z < maxRollAngle || model.transform.localEulerAngles.z > (360 - maxRollAngle)) {// if we haven't already rotated maxRollAngle degrees to make the turn
                if(deltaXZAngle < 180) {
                    model.transform.Rotate(Vector3.forward * (deltaXZAngle) * Time.deltaTime* 50);
                }
                else {
                    model.transform.Rotate(Vector3.forward * (deltaXZAngle - 360) * Time.deltaTime * 50);
                }
            }
        }
        // rotate back
        if (model.transform.localEulerAngles.z < 180) {
            model.transform.Rotate(-Vector3.forward * model.transform.localEulerAngles.z * Time.deltaTime * 10);
        }
        else {
            model.transform.Rotate(Vector3.forward * (360 - model.transform.localEulerAngles.z) * Time.deltaTime * 10);
        }
    }

    void Glide() {
        // get the component of velocity in the direction perpendicular to the glider (which is based on the orientation of the model)
        Vector3 gliderNormalForce = model.transform.up * Mathf.Pow(Mathf.Abs(Vector3.Dot(transform.up, rb.velocity)), airFrictionExponent) * airFrictionCoefficient * Mathf.Sign(Vector3.Dot(-model.transform.up, rb.velocity));

        // Debug.Log(gliderNormalForce + " : " + rb.velocity);

        Debug.DrawLine(model.transform.position, model.transform.position + gliderNormalForce, Color.blue, 0.1f);
        rb.AddForce(gliderNormalForce);

        if(Input.GetKey(KeyCode.Space) || Input.GetKey("joystick button 0")) {
            rb.AddForce(model.transform.forward * 50);
            trail.emitting = true;
        }
        else {
            trail.emitting = false;
        }
        // rb.velocity += gliderNormalForce * Time.deltaTime;
    }

    void OnCollisionEnter(Collision other) {
        if(PV.IsMine) {
            if(other.gameObject.GetComponent<PlayerController>()) {
                Debug.Log(GameManager.Instance);
                GameManager.Instance.RespawnPlayers();
            }
        }
    }

    public void Respawn()
    {
        PV.RPC("RPC_Respawn", RpcTarget.All);
        // tell this character controller to respawn
    }
    [PunRPC] void RPC_Respawn()
    {
        if(!PV.IsMine)
            return;

        playerManager.Respawn();
    }
    void Teleportable.Teleport(Vector3 destination) {
        Debug.Log("Instant Transmission!");
        Vector3 vel = rb.velocity;
        rb.isKinematic = true;
        transform.position = destination;
        rb.isKinematic = false;
        rb.velocity = vel;
    }
}
