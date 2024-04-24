using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCheckpoint : MonoBehaviour
{
    #region variables

    [Tooltip("The ghost replay script for the best lap")]
    [SerializeField] private GhostPlayer ghostPlayer1;
    [Tooltip("The ghost replay script for the last lap")]
    [SerializeField] private GhostPlayer ghostPlayer2;
    [Tooltip("The ScriptableObject that is going to store the fastests driven data")]
    [SerializeField] private GhostData bestData;
    [Tooltip("The ScriptableObject that is going to store the current driven data")]
    [SerializeField] private GhostData tempData;
    [Tooltip("The ScriptableObject that is going to store the current driven data")]
    [SerializeField] private GhostData tempData2;
    [Tooltip("The amount of times that the player is tracked per second")]
    [SerializeField] private float recordFrequency;

    private GhostData prevData;

    #endregion

    #region start

    public void Start()
    {
        tempData.ResetData();
        tempData2.ResetData();
    }

    #endregion

    #region collision

    public void OnTriggerEnter(Collider other)
    {
        ToggleRecorder(other);
        ToggleGhostPlayers(other);
    }

    #endregion

    #region recorder

    public void ToggleRecorder(Collider other)
    {
        GhostRecorder recorder = other.GetComponentInChildren<GhostRecorder>();

        if (recorder == null)
        {
            return;
        }

        if (recorder.StopRecording() == tempData)
        {
            prevData = tempData;
            tempData2.ResetData();
            recorder.StartRecording(tempData2, recordFrequency);
        }
        else
        {
            prevData = tempData2;
            tempData.ResetData();
            recorder.StartRecording(tempData, recordFrequency);
        }

        if (tempData2.totalTime < bestData.totalTime)
        {
            bestData = tempData2;
        }
    }

    #endregion

    #region ghost players

    public void ToggleGhostPlayers(Collider other)
    {
        if (bestData != null)
        {
            ghostPlayer1.StopPlaying();
            ghostPlayer1.StartPlaying(bestData);
        }

        if (prevData != null)
        {
            ghostPlayer2.StopPlaying();
            ghostPlayer2.StartPlaying(prevData);
        }
    }

    #endregion
}
