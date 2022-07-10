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
    [SerializeField] float maxRollAngle, rollStrength, rollSmoothness;
    
    TrailRenderer trail;
    PhotonView PV;
    PlayerManager playerManager;
    Vector3 gravityVector;
    float rollAngle = 0;
    Vector3 gliderNormalForce;
    

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
        RollSine();
        Glide();
        Gravity();
    }

    public Vector3 GetGliderNormalForce() {
        return gliderNormalForce;
    }

    public Vector3 GetVelocity() {
        return rb.velocity;
    }

    public Vector3 GetWorldHorizVel() {
        // horizontal relative to the world
        return rb.velocity - (Vector3.up * Vector3.Dot(rb.velocity, Vector3.up));
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



    void RollSine() {
        // Roll. Only roll the model
        // get difference between desired and actual horizontal angle (angle along the xz-plane). Roll depending on that
        float desiredXZAngle = Mathf.Atan(transform.forward.z/(transform.forward.x + 1e-9f)) + ((transform.forward.x < 0) ? Mathf.PI : 0);
        float actualXZAngle = Mathf.Atan(rb.velocity.z/(rb.velocity.x + 1e-9f)) + ((rb.velocity.x < 0) ? Mathf.PI : 0);

        float deltaXZAngle = (desiredXZAngle - actualXZAngle) / Mathf.PI * 180;
        deltaXZAngle = (deltaXZAngle % 360f + 360f) % 360f; // modulo

        // Debug.Log(deltaXZAngle);

        if (((deltaXZAngle > 0f  && deltaXZAngle < 180 - 0f) || (deltaXZAngle > 180 + 0f  && deltaXZAngle < 360 - 0f))  && (GetWorldHorizVel().magnitude / GetVelocity().magnitude) > 0.4 && (GetWorldHorizVel().magnitude > 10f)) {
            float deltaAngleSin = Mathf.Sin(deltaXZAngle / 180f * Mathf.PI);
            // rollAngle = Mathf.Pow(Mathf.Abs(deltaAngleSin), 0.2f) * Mathf.Sign(deltaAngleSin) * maxRollAngle;
            // Debug.Log("sine of angle" + deltaAngleSin);
            float desiredRollAngle = Mathf.Clamp(deltaAngleSin * rollStrength, -maxRollAngle, maxRollAngle);
            rollAngle += (desiredRollAngle - rollAngle) * Time.deltaTime * (1/rollSmoothness);
        }
        else {
            rollAngle = rollAngle * Mathf.Pow(0.1f, Time.deltaTime);
        }
        
        Vector3 newLocalEulerAngles = model.transform.localEulerAngles;
        newLocalEulerAngles.z = rollAngle;
        model.transform.localEulerAngles = newLocalEulerAngles;
    }

    void Roll() {
        // Roll. Only roll the model
        // get difference between desired and actual horizontal angle (angle along the xz-plane). Roll depending on that
        float desiredXZAngle = Mathf.Atan(transform.forward.z/(transform.forward.x + 1e-9f)) + ((transform.forward.x < 0) ? Mathf.PI : 0);
        float actualXZAngle = Mathf.Atan(rb.velocity.z/(rb.velocity.x + 1e-9f)) + ((rb.velocity.x < 0) ? Mathf.PI : 0);

        float deltaXZAngle = (desiredXZAngle - actualXZAngle) / Mathf.PI * 180;
        deltaXZAngle = (deltaXZAngle % 360f + 360f) % 360f; // modulo
        // while(deltaXZAngle > 360f) {
        //     deltaXZAngle -= 360f;
        // }
        // while(deltaXZAngle < 0f) {
        //     deltaXZAngle += 360f;
        // }

        if (((deltaXZAngle > 2  && deltaXZAngle < 180 - 2) || (deltaXZAngle > 180 + 2  && deltaXZAngle < 360 - 2))  && rb.velocity.magnitude > 10) {
            // roll to make the turn, but only if we are far away from the desired angle and we have enough speed
            if(model.transform.localEulerAngles.z > maxRollAngle && model.transform.localEulerAngles.z < (360 - maxRollAngle))
                Debug.Log("bad angle");
            // if(model.transform.localEulerAngles.z < maxRollAngle || model.transform.localEulerAngles.z > (360 - maxRollAngle)) {
                // if we haven't already rotated maxRollAngle degrees to make the turn
            if(deltaXZAngle < 180) {
                rollAngle += (deltaXZAngle) * Time.deltaTime * GetVelocity().magnitude / 2;
                // model.transform.Rotate(Vector3.forward * );
            }
            else {
                rollAngle += (deltaXZAngle - 360) * Time.deltaTime * GetVelocity().magnitude / 2;
                // model.transform.Rotate(Vector3.forward * (deltaXZAngle - 360) * Time.deltaTime * 50);
            }
            // rollAngle = (rollAngle % 360f + 360f) % 360f; // modulo
            // Debug.Log(rollAngle)

            // don't go past maximum roll angle
            rollAngle = Mathf.Clamp(rollAngle, -maxRollAngle, maxRollAngle);
            // if(maxRollAngle < rollAngle && rollAngle <= 180f) {
            //     rollAngle = maxRollAngle;
            // }
            // if(rollAngle < (360f - maxRollAngle) && rollAngle > 180f) {
            //     rollAngle = 360f - maxRollAngle;
            // }

            // }
        }
        else {
            // rotate back
            if (rollAngle > 0) {
                rollAngle -= model.transform.localEulerAngles.z * Time.deltaTime * 1;
                // model.transform.Rotate(-Vector3.forward * model.transform.localEulerAngles.z * Time.deltaTime * 2);
            }
            else {
                rollAngle += (360 - model.transform.localEulerAngles.z) * Time.deltaTime * 1;
                // model.transform.Rotate(Vector3.forward * (360 - model.transform.localEulerAngles.z) * Time.deltaTime * 2);
            }
        }
        
        // Debug.Log(rollAngle);

        Vector3 newLocalEulerAngles = model.transform.localEulerAngles;
        newLocalEulerAngles.z = rollAngle;
        model.transform.localEulerAngles = newLocalEulerAngles;
    }

    void Glide() {
        // get the component of velocity in the direction perpendicular to the glider (which is based on the orientation of the model)
        gliderNormalForce = model.transform.up * Mathf.Pow(Mathf.Abs(Vector3.Dot(transform.up, rb.velocity)), airFrictionExponent) * airFrictionCoefficient * Mathf.Sign(Vector3.Dot(-model.transform.up, rb.velocity));

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
    public void Teleport(Vector3 destination) {
        Debug.Log("Instant Transmission!");
        Vector3 vel = rb.velocity; // probably doesnt do anything
        rb.isKinematic = true; // probably doesnt do anything
        transform.position = destination;
        rb.isKinematic = false; // probably doesnt do anything
        rb.velocity = vel; // probably doesnt do anything
    }
}
