using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    #region variables

    private GhostData dataBank;
    private bool isPlaying;
    private float timer;

    //these are used to interpolate between these 2 vallues
    private int index1;
    private int index2;

    #endregion

    #region update

    public void Update()
    {
        GhostDriver();
        ChangeTransform();
    }

    #endregion

    #region toggle playing

    public void StartPlaying(GhostData data)
    {
        dataBank = data;
        
        if (dataBank == null)
        {
            return;
        }

        if (dataBank.timeStamps.Count == 0)
        {
            return;
        }

        timer = 0;
        transform.position = dataBank.positions[0];
        transform.eulerAngles = dataBank.rotations[0];
        isPlaying = true;
    }

    public void StopPlaying()
    {
        dataBank = null;
        isPlaying = false;
    }

    #endregion

    #region  ghost driver

    public void GhostDriver()
    {
        if (!isPlaying)
        {
            return;
        }

        timer += Time.unscaledDeltaTime;

        for (int i = 0; i < dataBank.timeStamps.Count - 2; i++)
        {
            //check in the if statement if the time is even with the timestamp. if so you dont need to interpolate
            if (dataBank.timeStamps[i] == timer)
            {
                index1 = i;
                index2 = i;
            }
            else if (dataBank.timeStamps[i] < timer && timer < dataBank.timeStamps[i + 1])
            {
                index1 = i;
            }
        }
    }

    #endregion

    #region apply to transform

    public void ChangeTransform()
    {
        if(!isPlaying)
        {
            return;
        }

        if (index1 == index2)
        {
            transform.position = dataBank.positions[index1];
            transform.eulerAngles = dataBank.rotations[index1];
        }
        else
        {
            float interpolateDelta = (timer - dataBank.timeStamps[index1]) / (dataBank.timeStamps[index1] - dataBank.timeStamps[index2]);

            transform.position = Vector3.Lerp(dataBank.positions[index1], dataBank.positions[index2], interpolateDelta);
            transform.eulerAngles = Vector3.Lerp(dataBank.rotations[index1], dataBank.rotations[index2], interpolateDelta);
        }
    }

    #endregion
}
