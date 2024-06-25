using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    #region variables

    [Description("This should be placed on the car")]

    private GhostData dataBank;
    private bool isRecording;
    private float recordFrequency;
    private float totalTime;
    private float frequencyTimer;

    #endregion

    #region Update

    public void Update()
    {
        Recorder();
    }

    #endregion

    #region toggle recording

    public void StartRecording(GhostData data, float frequency)
    {
        recordFrequency = frequency;
        dataBank = data;
        isRecording = true;
    }

    public GhostData StopRecording()
    {
        isRecording = false;
        GhostData data = new GhostData();

        if (dataBank != null)
        {
            dataBank.totalTime = totalTime;
            data = dataBank;
        }
        
        dataBank = null;
        totalTime = 0;
        frequencyTimer = 0;

        return data;
    }

    #endregion

    #region recorder

    public void Recorder()
    {
        if (!isRecording)
        {
            return;
        }

        totalTime += Time.unscaledDeltaTime;
        frequencyTimer += Time.unscaledDeltaTime;

        if (frequencyTimer > 1 / recordFrequency)
        {
            dataBank.timeStamps.Add(totalTime);
            dataBank.positions.Add(transform.position);
            dataBank.rotations.Add(transform.eulerAngles);
        }
    }

    #endregion
}
