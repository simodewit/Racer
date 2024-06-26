using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackSelect
{
    nationals,
    nationals2,
    GP,
}

public class TrackSelection : MonoBehaviour
{
    [SerializeField] private TrackInfo[] trackInfo;

    private void Start ( )
    {
        ChangeTrack (GetTrackSelectionFromName (Player.SavedData.SelectedMapSize));
    }

    TrackSelect GetTrackSelectionFromName(string name )
    {
        return name switch
        {
            "Nationals" => TrackSelect.nationals,
            "Nationals 2" => TrackSelect.nationals2,
            "Grand Prix" => TrackSelect.GP,
            _ => TrackSelect.nationals2
        };
    }

    public void ChangeTrack(TrackSelect selection)
    {
        foreach (TrackInfo info in trackInfo)
        {
            if (info.selection == selection)
            {
                info.objectsToEnable.SetActive(true);
            }
            else
            {
                info.objectsToEnable.SetActive(false);
            }
        }
    }
}

[Serializable]
public class TrackInfo
{
    public TrackSelect selection;
    public GameObject objectsToEnable;
}