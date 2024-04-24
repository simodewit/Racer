using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostData", menuName = "Ghost/GhostData", order = 1)]
public class GhostData : ScriptableObject
{
    public float totalTime;
    public List<float> timeStamps;
    public List<Vector3> positions;
    public List<Vector3> rotations;
        
    public void ResetData()
    {
        totalTime = 0;
        timeStamps.Clear();
        positions.Clear();
        rotations.Clear();
    }
}
