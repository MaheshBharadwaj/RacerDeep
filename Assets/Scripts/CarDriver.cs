using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarDriver : MonoBehaviour {

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    public int carId;
    private Rigidbody carRigidbody;
    #region Fields
    private float speed;
    [SerializeField] private float speedMax = 50f;
    private float speedMin = -10f;
    private float acceleration = 30f;
    private float brakeSpeed = 40f;
    private float reverseSpeed = 0f;
    private float idleSlowdown = 10f;

    private float turnSpeed;
    private float turnSpeedMax = 300f;
    private float turnSpeedAcceleration = 300f;
    private float turnIdleSlowdown = 500f;

    private float forwardAmount;
    private float turnAmount;

    // private Rigidbody carRigidbody;
    #endregion

   

    private void Awake() {
        //Debug.Log("Driver Car ID: " + carId + " Track Checkpoints carID: " + trackCheckpoints.GetCarID());
        carRigidbody = GetComponent<Rigidbody>();
    }

    public int getCarID()
    {
        return carId;
    }

    private void FixedUpdate() {
        if (forwardAmount > 0) {
            // Accelerating
            speed += forwardAmount * acceleration * Time.deltaTime;
        }
        if (forwardAmount < 0) {
            if (speed > 0) {
                // Braking
                speed += forwardAmount * brakeSpeed * Time.deltaTime;
            } else {
                // Reversing
                speed += forwardAmount * reverseSpeed * Time.deltaTime;
            }
        }

        if (forwardAmount == 0) {
            // Not accelerating or braking
            if (speed > 0) {
                speed -= idleSlowdown * Time.deltaTime;
            }
            if (speed < 0) {
                speed += idleSlowdown * Time.deltaTime;
            }
        }

        speed = Mathf.Clamp(speed, speedMin, speedMax);

        carRigidbody.velocity = transform.forward * speed;

        if (speed < 0) {
            // Going backwards, invert wheels
            turnAmount = turnAmount * -1f;
        }

        if (turnAmount > 0 || turnAmount < 0) {
            // Turning
            if ((turnSpeed > 0 && turnAmount < 0) || (turnSpeed < 0 && turnAmount > 0)) {
                // Changing turn direction
                float minTurnAmount = 20f;
                turnSpeed = turnAmount * minTurnAmount;
            }
            turnSpeed += turnAmount * turnSpeedAcceleration * Time.deltaTime;
        } else {
            // Not turning
            if (turnSpeed > 0) {
                turnSpeed -= turnIdleSlowdown * Time.deltaTime;
            }
            if (turnSpeed < 0) {
                turnSpeed += turnIdleSlowdown * Time.deltaTime;
            }
            if (turnSpeed > -1f && turnSpeed < +1f) {
                // Stop rotating
                turnSpeed = 0f;
            }
        }

        float speedNormalized = speed / speedMax;
        float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

        turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);

        carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 || transform.eulerAngles.z < -2) {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    //private void OnCollisionEnter(Collision collision) {
    //    if (collision.gameObject.tag == "SideWall") {
    //        transform.localPosition = new Vector3(0.0f, 23f, 0.0f);
    //        transform.rotation = Quaternion.Euler(0, 90, 0);
    //        speed = 0.0f;
    //        turnSpeed = 0.0f;
    //        trackCheckpoints.resetNextCheckpointSingleIndex();
    //    }
    //}

    private void ResetCheckpoint(Transform transform) {
        // Debug.Log("RESET CHECKPOINT INSIDE CAR DRIVER");
    }

    public void SetInputs(float forwardAmount, float turnAmount) {
        this.forwardAmount = forwardAmount;
        this.turnAmount = turnAmount;
    }

    public void ClearTurnSpeed() {
        turnSpeed = 0f;
    }

    public float GetSpeed() {
        return speed;
    }


    public void SetSpeedMax(float speedMax) {
        this.speedMax = speedMax;
    }

    public void SetTurnSpeedMax(float turnSpeedMax) {
        this.turnSpeedMax = turnSpeedMax;
    }

    public void SetTurnSpeedAcceleration(float turnSpeedAcceleration) {
        this.turnSpeedAcceleration = turnSpeedAcceleration;
    }

    public void StopCompletely() {
        speed = 0f;
        turnSpeed = 0f;
    }

    public Vector3 GetRigidbodyVelocity() {
        return carRigidbody.velocity;
    }

}
