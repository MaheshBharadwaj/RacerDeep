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
        this.gameObject.tag = "IncorrectCheckpointTag";
        
        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            Debug.Log("Mesh Renderer Accesssible");
            Debug.Log(meshRenderer);
            //meshRenderer.materials
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarDriver>(out CarDriver carDriver))
        {
            trackCheckpoints.PlayerThroughCheckpoint(this, carDriver.getCarID());
        }
    }

    public void setTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
        this.carId = trackCheckpoints.GetCarID();
    }

    

}