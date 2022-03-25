using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    private int carId;

    void Awake()
    {
        // Debug.Log("Target Created");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarDriver>(out CarDriver carDriver))
        {
            trackCheckpoints.PlayerThroughCheckpoint(this, carDriver.carId);
        }
    }

    public void setTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
        this.carId = trackCheckpoints.GetCarID();
    }

    

}