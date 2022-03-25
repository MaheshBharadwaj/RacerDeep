using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public static TrackCheckpoints current;
    [SerializeField]
    public int carId;

    public event Action<int> OnPlayerCorrectCheckpoint;
    public void PlayerCorrectCheckpoint(int CarId)
    {
        if(OnPlayerCorrectCheckpoint != null)
        {
            OnPlayerCorrectCheckpoint(CarId);
        }
    }

    public event Action<int> OnPlayerWrongCheckpoint;
    public void PlayerWrongCheckpoint(int CarID)
    {
        if (OnPlayerWrongCheckpoint != null)
        {
            OnPlayerWrongCheckpoint(CarID);
        }
    }

    public event Action<int> OnPlayerLapCompleted;
    public void PlayerLapCompleted(int CarID)
    {
        if(OnPlayerLapCompleted != null)
        {
            OnPlayerLapCompleted(CarID);
        }
    }

    private List<CheckpointSingle> checkpointSingleList;
    private int nextCheckpointSingleIndex;
    //private List<int> nextCheckpointSingleIndex;
    private Boolean gameStarted = false;

    public void Awake()
    {
        
        current = this;
        Transform checkpointsTransform = this.transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.setTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
        }
        Debug.Log("number of checkpoints: " + checkpointSingleList.Count);
        //for(int i = 0; i < )
        //nextCheckpointSingleIndex = 0;
    }

    public void Initialize()
    {

    }


    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle, int carID)
    {
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            PlayerCorrectCheckpoint(carID);
            if (nextCheckpointSingleIndex == 0 && gameStarted)
            {
                PlayerLapCompleted(carID);
            }
            if (!gameStarted)
            {
                gameStarted = true;
            }
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
        }
        else
        {
            ;
            PlayerWrongCheckpoint(carID);

        }
    }

    public int GetCarID()
    {
        return carId;
    }

    public void resetNextCheckpointSingleIndex()
    {
        nextCheckpointSingleIndex = 0;
        gameStarted = false;
    }

    public void ResetCheckpoint()
    {
        nextCheckpointSingleIndex = 0;
        Debug.Log("RESET CHECKPOINT INSIDE TRACK CHECKPOINTS");
    }

    public CheckpointSingle GetNextCheckpoint(Transform transform)
    {
        int requiredCheckpointIndex = (nextCheckpointSingleIndex) % checkpointSingleList.Count;
        return checkpointSingleList[requiredCheckpointIndex];
    }

}   