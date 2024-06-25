using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceType : MonoBehaviour
{
    [Tooltip("The amount of grip on this surface in percentages"), Range(0f, 100f)]
    public float grip = 100;
    [Tooltip("The amount of resistance when driving on this surface"), Range(0, 1000)]
    public float resistance = 100;
    [Tooltip("The minimum amount of resistance"), Range(0, 1000)]
    public float minimumResistance = 20;
}
