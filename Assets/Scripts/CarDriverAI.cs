using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class CarDriverAI : Agent
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private int carId;
    private TrailRenderer trailRenderer;
    //public int MaxStep;
    private Vector3 initalPosition;
    private Quaternion initialRotation;
    private Vector3 goal;
    private string checkpointSingle;
    private CarDriver carDriver;
    private int counter = 0;


    private void Awake()
    {
        //Debug.Log("AI Car ID: " + carId + " Track Checkpoints carID: " + trackCheckpoints.GetCarID());
        //carDriver = GetComponent<CarDriver>();
        Transform playerTransform = trackCheckpoints.transform.Find("Player");
        carDriver = playerTransform.GetComponent<CarDriver>();
        //carId = carDriver.getCarID();
        goal = new Vector3(75f, 23.09f, 55.2f);
        trackCheckpoints.OnPlayerCorrectCheckpoint += Agent_onPlayerCorrectCheckpoint;
        trackCheckpoints.OnPlayerWrongCheckpoint += Agent_onPlayerWrongCheckpoint;
        trackCheckpoints.OnPlayerLapCompleted += Agent_onPlayerLapCompleted;

        trailRenderer = gameObject.GetComponent(typeof(TrailRenderer)) as TrailRenderer;
    }
    private void CheckRewardThresh()
    {
        if (GetCumulativeReward() <= -1)
        {
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
            trailRenderer.Clear();
        }
    }


    public void Agent_onPlayerCorrectCheckpoint(int carId)
    {
        if (this.carId == carId)
        {
            Debug.Log("Correct Checkpoint: +1f");
            //if (GetCumulativeReward() < 0)
            //    SetReward(+1f);
            //else
            //    AddReward(+1f);
            AddReward(+1f);

        }
        else
        {
            Debug.Log("Not my business" + carId + " " + this.carId);
        }

    }
    public void Agent_onPlayerWrongCheckpoint(int carId)
    {
        if (this.carId == carId)
        {
            Debug.Log("Incorrect Checkpoint: -1f");
            AddReward(-1f);
            CheckRewardThresh();
        }
        else
        {
            Debug.Log("Not my business");
        }

    }

    public void Agent_onPlayerLapCompleted(int carId)
    {
        if (this.carId == carId)
        {
            Debug.Log("Lap Completed: +20f");
            AddReward(+40f);
            EndEpisode();
        }
        else
        {
            Debug.Log("Not my business");
        }
    }

    public override void Initialize()
    {


        //MaxStep = 10000;
        // Debug.Log("SPAWN POSITION POSITION");
        // Debug.Log(spawnPosition.localPosition + new Vector3((float) 20.0, (float) 0.0, (float) 0.0));

        //goal = spawnPosition.localPosition + new Vector3((float)30.0, (float)0.0, (float)0.0);
        //checkpointSingle = trackCheckpoints.GetNextCheckpoint(this.transform);
        checkpointSingle = "CheckpointSingle";
        initalPosition = transform.localPosition;
        initialRotation = transform.rotation;

    }

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3( -16.6f, 23.09f, -1.9f) + new Vector3(Random.Range(-5f, +5f), 0, Random.Range(-5f, +5f));
        transform.localPosition = initalPosition;
        transform.rotation = initialRotation;
        //Debug.Log("Reset position: " + transform.localPosition);
        // trackCheckpoints.ResetCheckpoint(transform);
        trackCheckpoints.ResetCheckpoint();
        trackCheckpoints.resetNextCheckpointSingleIndex();
        carDriver.StopCompletely();
        SetReward(0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Debug.Log("Rotation[0]: " + transform.rotation[0]);
        //Debug.Log("Rotation[1]: " + transform.rotation[1]);
        //Debug.Log("Rotation[2]: " + transform.rotation[2]);
        //Debug.Log("Rotation[3]: " + transform.rotation[3]);
        // Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        AddReward((-1f / MaxStep)*0.1f);
        sensor.AddObservation(new Vector3(transform.localPosition[0], transform.localPosition[1], transform.localPosition[2]));
        sensor.AddObservation(carDriver.GetSpeed());
        sensor.AddObservation(transform.rotation[1]);
        sensor.AddObservation(transform.rotation[3]);
    }


    public float euclideanDistance(Vector3 pos1, Vector3 pos2)
    {
        float distance = 0;

        distance += (pos1[0] - pos2[0]) * (pos1[0] - pos2[0]);
        distance += (pos1[1] - pos2[1]) * (pos1[1] - pos2[1]);
        distance += (pos1[2] - pos2[2]) * (pos1[2] - pos2[2]);

        return Mathf.Sqrt(distance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
        //AddReward(-0.1f);

        //Debug.Log("actions[0]: " + actions.DiscreteActions[0] + " actions[1]: " + actions.DiscreteActions[1]);
        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: if (carDriver.GetSpeed() > 0) forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }


        carDriver.SetInputs(forwardAmount, turnAmount);
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (checkpointSingle.Equals(other.gameObject.name))
    //    {
    //        Debug.Log("Correct Checkpoint: +10F");
    //        AddReward(1f);
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect Checkpoint: -10F");
    //        AddReward(-1f);
    //        CheckRewardThresh();
    //    }
    //    checkpointSingle = trackCheckpoints.GetNextCheckpoint(this.transform).gameObject.name;

    //}

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("WE GOT A COLLISION");
        AddReward(-0.02f * carDriver.GetSpeed());
        CheckRewardThresh();
        //EndEpisode();

    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Counter: " + counter);
        counter += 1;
        AddReward(-0.02f * carDriver.GetSpeed() - 0.2f);
        //CheckRewardThresh();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Debug.Log("ACTIONS RECIEVED");

        int forwardAction = 0;
        if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 1;
        if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 2;

        int turnAction = 0;
        if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) turnAction = 2;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;

    }

}
