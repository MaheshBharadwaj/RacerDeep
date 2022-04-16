using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    [SerializeField] private Material correctCheckpointMaterial;
    [SerializeField] private Material incorrectCheckpointMaterial;
    public static TrackCheckpoints current;
    private int carId;
    public void Awake()
    {

        //Debug.Log("Im printing the NAME");
        //Debug.Log(this.name.Substring(this.name.Length - 5, this.name.Length - 1));

        carId = getIDNumber(this.name);
        Debug.Log("My name is " + this.name);
        Debug.Log("My car ID is " + carId);
        
        current = this;
        Transform checkpointsTransform = this.transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.setTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
            if (checkpointSingle.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                //Debug.Log(meshRenderer);
                meshRenderer.material = incorrectCheckpointMaterial;
            }
        }
        Debug.Log("number of checkpoints: " + checkpointSingleList.Count);
        //for(int i = 0; i < )
        //nextCheckpointSingleIndex = 0;
    }

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
            // This checkpoint is incorrect now
            checkpointSingleList[nextCheckpointSingleIndex].gameObject.tag = "IncorrectCheckpointTag";
            if (checkpointSingleList[nextCheckpointSingleIndex].gameObject.TryGetComponent(out MeshRenderer meshRendererOld))
            {
                meshRendererOld.material = incorrectCheckpointMaterial;
            }
            // Obtain next checkpoint index
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            // Set the tag for correct checkpoint and material for correct checkpoint
            checkpointSingleList[nextCheckpointSingleIndex].gameObject.tag = "CorrectCheckpointTag";
            if (checkpointSingleList[nextCheckpointSingleIndex].gameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = correctCheckpointMaterial;
            }
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
        foreach (CheckpointSingle checkpointSingle in checkpointSingleList)
        {
            checkpointSingle.gameObject.tag = "IncorrectCheckpointTag";
            if (checkpointSingle.gameObject.TryGetComponent(out MeshRenderer meshRenderer2))
            {
                
                meshRenderer2.material = incorrectCheckpointMaterial;
            }
        }
        checkpointSingleList[0].gameObject.tag = "CorrectCheckpointTag";
        if (checkpointSingleList[0].gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.material = correctCheckpointMaterial;
        }
    }

    public float LapCompletedPercentage()
    {
        Debug.Log("Lap progress: " + ((float)nextCheckpointSingleIndex / (float)checkpointSingleList.Count) * 100f);
        return ((float)nextCheckpointSingleIndex / (float)checkpointSingleList.Count) * 100f;
    }

    public CheckpointSingle GetNextCheckpoint(Transform transform)
    {
        int requiredCheckpointIndex = (nextCheckpointSingleIndex) % checkpointSingleList.Count;
        return checkpointSingleList[requiredCheckpointIndex];
    }

    public static int getIDNumber(String name)
    {
        String strRegex = @"\(\d+\)$";
        //Debug.Log(this.name);
        Regex re = new Regex(strRegex);
        Match match = re.Match(name);
        int number = 0;


        if (match.Success)
        {
            int length = match.Value.Length;
            //Debug.Log("the number is " + match.Value.Substring(1, length-2));
            number = int.Parse(match.Value.Substring(1, length - 2));
        }

        return number;

    }

}   