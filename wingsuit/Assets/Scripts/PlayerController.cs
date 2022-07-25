using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, Teleportable, IPunObservable
{
    [SerializeField] GameObject cameraHolder;
    // [SerializeField] float controllerSensitivity;
    // [SerializeField] float sensitivity;
    [SerializeField] float gravity;
    [SerializeField] GameObject rollable;
    [SerializeField] float maxRollAngle, rollStrength, rollSmoothness;
    
    TrailRenderer trail;
    public PhotonView PV;
    PlayerManager playerManager;
    Vector3 gravityVector;
    float rollAngle = 0;
    Vector3 gliderNormalForce;
    bool isFrozen = false;

    // private Vector3 globalVelocity = new Vector3();

    private Rigidbody rb;

    private float verticalLookRotation;

    [SerializeField] private float airFrictionCoefficient;
    [SerializeField] private float airFrictionExponent;
    [SerializeField] public float maxBoostAmount;
    [HideInInspector] public float boostRemaining;
    [SerializeField] float boostRegenRate;

    // Elytra Parameters
    [SerializeField] float elyGravity;
    [SerializeField] float normalForce;
    [SerializeField] float vertToHorz;
    [SerializeField] float horizToVert;
    [SerializeField] float horizToHoriz;
    [SerializeField] float vertDecay;
    [SerializeField] float horizDecay;

    void Awake() {
        boostRemaining = maxBoostAmount;
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        // trail = GetComponent<TrailRenderer>();
        gravityVector = new Vector3(0, -gravity, 0);
    }

    void Start() {
        if(!PV.IsMine) {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            // Destroy(rb);
        }
        else {
            // disable your own trail for yourself
            // GetComponent<TrailRenderer>().enabled = false;
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
    }

    void FixedUpdate() {
        if(!PV.IsMine) {
            return;
        }

        if(!isFrozen) {
            rb.isKinematic = false;
            // Glide();
            // Gravity();
            GlideElytraWithParameters();
            Boost();
        }
        else {
            rb.isKinematic = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // sync position and rotation and do lag compensation
        if (stream.IsWriting)
        {
            // Debug.Log("writing" + PV.Owner.NickName + " " + rb.position + " " + transform.rotation);
            // stream.SendNext(rb.position);
            stream.SendNext(transform.rotation.eulerAngles);
            stream.SendNext(rb.velocity);
            stream.SendNext(rollable.transform.eulerAngles);
        }
        else
        {
            // Debug.Log("reading" + PV.Owner.NickName);
            // rb.position = (Vector3) stream.ReceiveNext();
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, (Vector3) stream.ReceiveNext(), 0.1f);
            rb.velocity = (Vector3) stream.ReceiveNext();
            rollable.transform.eulerAngles = (Vector3) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            rb.position += rb.velocity * lag;
        }
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

    // void Look() {
    //     transform.Rotate(-Vector3.forward * Input.GetAxisRaw("Horizontal") * Time.deltaTime * controllerSensitivity);

    //     transform.Rotate(Vector3.right * Input.GetAxisRaw("Vertical") * Time.deltaTime * controllerSensitivity);
    //     // transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * Time.deltaTime * keyboardSensitivity / 1.5f);
    // }

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
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * SettingsManager.Instance.mouseSensitivity, Space.World);
        // transform.Rotate(Vector3.up,)
        // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);
        
        // Pitch *not really because world space
        transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * SettingsManager.Instance.mouseSensitivity);
    }

    void ControllerLook() {
        // Yaw *not really because world space
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * SettingsManager.Instance.controllerSensitivity, Space.World);
        // transform.Rotate(Vector3.up,)
        // transform.RotateAround(transform.InverseTransformVector(Vector3.up), Input.GetAxisRaw("Mouse X") * sensitivity);
        
        // Pitch *not really because world space
        transform.Rotate(Vector3.right * Input.GetAxisRaw("Vertical") * SettingsManager.Instance.controllerSensitivity);
    }



    void RollSine() {
        // Roll. Only roll the stuff under the rollable gameobject
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
        
        Vector3 newLocalEulerAngles = rollable.transform.localEulerAngles;
        newLocalEulerAngles.z = rollAngle;
        rollable.transform.localEulerAngles = newLocalEulerAngles;
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
            if(rollable.transform.localEulerAngles.z > maxRollAngle && rollable.transform.localEulerAngles.z < (360 - maxRollAngle))
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
                rollAngle -= rollable.transform.localEulerAngles.z * Time.deltaTime * 1;
                // model.transform.Rotate(-Vector3.forward * model.transform.localEulerAngles.z * Time.deltaTime * 2);
            }
            else {
                rollAngle += (360 - rollable.transform.localEulerAngles.z) * Time.deltaTime * 1;
                // model.transform.Rotate(Vector3.forward * (360 - model.transform.localEulerAngles.z) * Time.deltaTime * 2);
            }
        }
        
        // Debug.Log(rollAngle);

        Vector3 newLocalEulerAngles = rollable.transform.localEulerAngles;
        newLocalEulerAngles.z = rollAngle;
        rollable.transform.localEulerAngles = newLocalEulerAngles;
    }

    void Glide() {
        if(Input.GetKey(KeyCode.LeftShift)) {
            // Debug.Log("here");
            return;
        }
        // get the component of velocity in the direction perpendicular to the glider (which is based on the orientation of the "rollable" game object)
        gliderNormalForce = rollable.transform.up * Mathf.Pow(Mathf.Abs(Vector3.Dot(transform.up, rb.velocity)), airFrictionExponent) * airFrictionCoefficient * Mathf.Sign(Vector3.Dot(-rollable.transform.up, rb.velocity));

        // Debug.Log(gliderNormalForce + " : " + rb.velocity);

    
        Debug.DrawLine(rollable.transform.position, rollable.transform.position + gliderNormalForce, Color.blue, 0.1f);
        rb.AddForce(gliderNormalForce);
        // rb.velocity += gliderNormalForce * Time.deltaTime;
    }

    void GlideElytraWorking() {
        // defining values
        Vector3 moveVecPerFrame = rb.velocity * Time.fixedDeltaTime;

        float pitch = transform.eulerAngles.x / 180f * Mathf.PI;
        float yaw = -transform.eulerAngles.y / 180f * Mathf.PI;

        float yawcos = Mathf.Cos(yaw - Mathf.PI);
        float yawsin = Mathf.Sin(yaw);
        float pitchcos = Mathf.Cos(pitch);
        float pitchsin = Mathf.Sin(pitch);

        float lookX = yawsin * (-pitchcos); // probably actually lookZ
        Debug.Log("lookX new" + lookX);
        float lookY = -pitchsin; // might be positive
        float lookZ = yawcos * (-pitchcos); // might be lookX
        Debug.Log("lookZ new" + lookZ);

        float hvel = (new Vector3(moveVecPerFrame.x, 0, moveVecPerFrame.z)).magnitude;
        float hlook = pitchcos;
        float sqrpitchcos = pitchcos * pitchcos;

        // actually applying values
        moveVecPerFrame += new Vector3(0, -0.08f + Mathf.Cos(pitch) * Mathf.Cos(pitch) * 0.06f, 0);

        if (moveVecPerFrame.y < 0 && hlook > 0) {
            // moving towards ground and not looking directly up or down
            float yacc = moveVecPerFrame.y * -0.1f * sqrpitchcos;
            moveVecPerFrame += new Vector3(lookX * yacc / hlook, yacc, lookZ * yacc / hlook);
        }

        if (pitch < 0) {
            float yacc = hvel * -pitchsin * 0.04f;
            moveVecPerFrame += new Vector3(lookX * yacc / hlook, yacc * 3.5f, lookZ * yacc / hlook);
        }

        if (hlook > 0) {
            moveVecPerFrame += new Vector3((lookX / hlook * hvel - moveVecPerFrame.x) * 0.1f, 0, (lookZ / hlook * hvel - moveVecPerFrame.z) * 0.1f);
            // this.velX += (lookX / hlook * hvel - this.velX) * 0.1;
			// this.velZ += (lookZ / hlook * hvel - this.velZ) * 0.1;
        }

        moveVecPerFrame *= 0.999f; // TODO: change this to same values as code if not good
        Debug.Log("per frame" + moveVecPerFrame);
        rb.velocity = moveVecPerFrame / Time.fixedDeltaTime;
        Debug.Log(rb.velocity);
        // rb.velocity *= f;
        // rb.velocity /= Time.deltaTime / 1000;
    }

    void GlideElytraReadable() {
        // defining values
        Vector3 moveVecPerFrame = rb.velocity * Time.fixedDeltaTime;

        float pitch = (360-transform.eulerAngles.x) / 180f * Mathf.PI;
        float yaw = transform.eulerAngles.y / 180f * Mathf.PI;

        float yawcos = Mathf.Cos(yaw);
        float yawsin = Mathf.Sin(yaw);
        float pitchcos = Mathf.Cos(pitch);
        float pitchsin = Mathf.Sin(pitch);

        float lookX = yawsin * pitchcos; // probably actually lookZ
        // Debug.Log("lookX" + lookX);
        float lookY = pitchsin; // might be positive
        float lookZ = yawcos * pitchcos; // might be lookX
        // Debug.Log("lookZ" + lookZ);

        float hvel = (new Vector3(moveVecPerFrame.x, 0, moveVecPerFrame.z)).magnitude;
        float hlook = pitchcos;
        float sqrpitchcos = pitchcos * pitchcos;

        // actually applying values
        moveVecPerFrame += new Vector3(0, -0.08f + sqrpitchcos * 0.06f, 0);

        if (moveVecPerFrame.y < 0 && hlook > 0) {
            // moving towards ground and not looking directly up or down
            float yacc = moveVecPerFrame.y * -0.1f * sqrpitchcos; // normal force
            // converting vertical velocity into horizontal velocity
            moveVecPerFrame += new Vector3(lookX * yacc / hlook, yacc, lookZ * yacc / hlook);
        }

        if (pitchsin > 0) {
            // if pointing upward
            // converting horizontal velocity into vertical velocity
            float yacc = hvel * pitchsin * 0.04f;
            moveVecPerFrame += new Vector3(-lookX * yacc / hlook, yacc * 3.5f, -lookZ * yacc / hlook);
        }

        if (hlook > 0) {
            // horizontal acceleration based on difference between horizontal look and horizontal velocity
            moveVecPerFrame += new Vector3((lookX / hlook * hvel - moveVecPerFrame.x) * 0.1f, 0, (lookZ / hlook * hvel - moveVecPerFrame.z) * 0.1f);
            // this.velX += (lookX / hlook * hvel - this.velX) * 0.1;
			// this.velZ += (lookZ / hlook * hvel - this.velZ) * 0.1;
        }

        moveVecPerFrame *= .98f; // TODO: change this to same values as code if not good
        // Debug.Log("per frame" + moveVecPerFrame);
        rb.velocity = moveVecPerFrame / Time.fixedDeltaTime;
        // Debug.Log(rb.velocity);
        // rb.velocity *= f;
        // rb.velocity /= Time.deltaTime / 1000;
    }

    void GlideElytraForces() {
        // defining values
        // Vector3 moveVecPerFrame = rb.velocity * Time.fixedDeltaTime;

        float pitch = (360-transform.eulerAngles.x) / 180f * Mathf.PI;
        float yaw = transform.eulerAngles.y / 180f * Mathf.PI;

        float yawcos = Mathf.Cos(yaw);
        float yawsin = Mathf.Sin(yaw);
        float pitchcos = Mathf.Cos(pitch);
        float pitchsin = Mathf.Sin(pitch);

        float lookX = yawsin * pitchcos; // probably actually lookZ
        // Debug.Log("lookX" + lookX);
        float lookY = pitchsin; // might be positive
        float lookZ = yawcos * pitchcos; // might be lookX
        // Debug.Log("lookZ" + lookZ);

        float hvel = (new Vector3(rb.velocity.x, 0, rb.velocity.z)).magnitude;
        float hlook = pitchcos;
        float sqrpitchcos = pitchcos * pitchcos;

        // actually applying values
        // moveVecPerFrame += new Vector3(0, -0.08f + sqrpitchcos * 0.06f, 0);
        rb.velocity += new Vector3(0, -0.64f + sqrpitchcos * 0.48f, 0) * 100 * Time.fixedDeltaTime;

        if (rb.velocity.y < 0 && hlook > 0) {
            // moving towards ground and not looking directly up or down
            float yacc = rb.velocity.y * -0.1f * sqrpitchcos / 10; // normal force
            // converting vertical velocity into horizontal velocity
            rb.velocity += new Vector3(lookX * yacc / hlook, yacc, lookZ * yacc / hlook) * 100 * Time.fixedDeltaTime;
        }

        if (true) {
            // if pointing upward
            // converting horizontal velocity into vertical velocity
            float yacc = hvel * pitchsin * 0.8f / 100;
            rb.velocity += new Vector3(-lookX * yacc / hlook, yacc, -lookZ * yacc / hlook) * 100 * Time.fixedDeltaTime;
        }

        if (hlook > 0) {
            // horizontal acceleration based on difference between horizontal look and horizontal velocity
            rb.velocity += new Vector3((lookX / hlook * hvel - rb.velocity.x) * 0.1f, 0, (lookZ / hlook * hvel - rb.velocity.z) * 0.1f) * 100 * Time.fixedDeltaTime;
            // this.velX += (lookX / hlook * hvel - this.velX) * 0.1;
			// this.velZ += (lookZ / hlook * hvel - this.velZ) * 0.1;
        }


        Debug.Log(Mathf.Pow(Mathf.Pow(0.99f, 1), Time.fixedDeltaTime));
        Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        newVelocity.x *= Mathf.Pow(Mathf.Pow(0.99f, 1), Time.fixedDeltaTime);
        newVelocity.y *= Mathf.Pow(Mathf.Pow(0.98f, 1), Time.fixedDeltaTime);
        newVelocity.z *= Mathf.Pow(Mathf.Pow(0.99f, 1), Time.fixedDeltaTime);

        // Debug.Log("per frame" + moveVecPerFrame);
        rb.velocity = newVelocity;
        // Debug.Log(rb.velocity);
        // rb.velocity *= f;
        // rb.velocity /= Time.deltaTime / 1000;
    }

    void GlideElytraWithParameters() {
        // defining values
        // Vector3 moveVecPerFrame = rb.velocity * Time.fixedDeltaTime;

        float pitch = (360-transform.eulerAngles.x) / 180f * Mathf.PI;
        float yaw = transform.eulerAngles.y / 180f * Mathf.PI;

        float yawcos = Mathf.Cos(yaw);
        float yawsin = Mathf.Sin(yaw);
        float pitchcos = Mathf.Cos(pitch);
        float pitchsin = Mathf.Sin(pitch);

        float lookX = yawsin * pitchcos; // probably actually lookZ
        // Debug.Log("lookX" + lookX);
        float lookY = pitchsin; // might be positive
        float lookZ = yawcos * pitchcos; // might be lookX
        // Debug.Log("lookZ" + lookZ);

        float hvel = (new Vector3(rb.velocity.x, 0, rb.velocity.z)).magnitude;
        float hlook = pitchcos;
        float sqrpitchcos = pitchcos * pitchcos;

        // actually applying values
        // moveVecPerFrame += new Vector3(0, -0.08f + sqrpitchcos * 0.06f, 0);
        rb.velocity += new Vector3(0, -elyGravity + sqrpitchcos * normalForce, 0) * Time.fixedDeltaTime;

        if (rb.velocity.y < 0 && hlook > 0) {
            // moving towards ground and not looking directly up or down
            float yacc = rb.velocity.y * -vertToHorz * sqrpitchcos; // normal force
            // converting vertical velocity into horizontal velocity
            rb.velocity += new Vector3(lookX * yacc / hlook, yacc, lookZ * yacc / hlook) * Time.fixedDeltaTime;
        }

        if (true) {
            // if pointing upward
            // converting horizontal velocity into vertical velocity
            float yacc = hvel * pitchsin * horizToVert;
            rb.velocity += new Vector3(-lookX * yacc / hlook, yacc, -lookZ * yacc / hlook) * Time.fixedDeltaTime;
        }

        if (hlook > 0) {
            // horizontal acceleration based on difference between horizontal look and horizontal velocity
            rb.velocity += new Vector3(lookX / hlook * hvel - rb.velocity.x, 0, lookZ / hlook * hvel - rb.velocity.z) * horizToHoriz * Time.fixedDeltaTime;
            // this.velX += (lookX / hlook * hvel - this.velX) * 0.1;
			// this.velZ += (lookZ / hlook * hvel - this.velZ) * 0.1;
        }


        Debug.Log(Mathf.Pow(Mathf.Pow(0.99f, 1), Time.fixedDeltaTime));
        Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        newVelocity.x *= Mathf.Pow(horizDecay, Time.fixedDeltaTime);
        newVelocity.y *= Mathf.Pow(vertDecay, Time.fixedDeltaTime);
        newVelocity.z *= Mathf.Pow(horizDecay, Time.fixedDeltaTime);

        // Debug.Log("per frame" + moveVecPerFrame);
        rb.velocity = newVelocity;
        // Debug.Log(rb.velocity);
        // rb.velocity *= f;
        // rb.velocity /= Time.deltaTime / 1000;
    }


    void Boost() {
        if(boostRemaining > 0 && (Input.GetKey(KeyCode.Space) || Input.GetKey("joystick button 0"))) {
            rb.AddForce(rollable.transform.forward * 100);
            boostRemaining -= Time.deltaTime;
            // trail.emitting = true;
        }
        else {
            boostRemaining = Mathf.Min(boostRemaining + Time.deltaTime * boostRegenRate, maxBoostAmount);
            // trail.emitting = false;
        }
    }

    void OnTriggerStay(Collider other) {
        // Tag detection. See if you tagged someone else
        if(PV.IsMine) {
            // Debug.Log("triggered");
            if(other.gameObject.GetComponentInParent<PlayerController>()) {
                // Debug.Log("tagged player");
                if(GameManager.Instance.SyncedTagger == PhotonNetwork.LocalPlayer) {
                    // I am the tagger and I have tagged another player
                    Player otherPlayer = other.gameObject.GetComponentInParent<PhotonView>().Owner;
                    GameManager.Instance.ApplyTag(tagger: PhotonNetwork.LocalPlayer, otherPlayer);
                }
                // Debug.Log(GameManager.Instance);
                // GameManager.Instance.RespawnPlayers();
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

    public void MoveTo(Transform moveToTransform) {
        PV.RPC("RPC_MoveTo", RpcTarget.All, new object[] {moveToTransform.position, moveToTransform.rotation});
    }

    [PunRPC] void RPC_MoveTo(Vector3 moveToPosition, Quaternion moveToRotation) {
        if(PV.IsMine) {
            // rb.isKinematic = true;
            this.transform.position = moveToPosition;
            this.transform.rotation = moveToRotation;
        }
    }

    public void SetFrozen(bool isFrozen) {
        PV.RPC("RPC_SetFrozen", RpcTarget.All, new object[] {isFrozen});
    }
    [PunRPC] void RPC_SetFrozen(bool isFrozen) {
        if(PV.IsMine) {
            this.isFrozen = isFrozen;
        }
    }

    public void SetBoostPercentage(float boostPercentage) {
        PV.RPC("RPC_SetBoostPercentage", RpcTarget.All, new object[] {boostPercentage});
    }

    [PunRPC] void RPC_SetBoostPercentage(float boostPercentage) {
        if(PV.IsMine) {
            boostRemaining = boostPercentage * maxBoostAmount;
        }
    }
}
