using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyres : MonoBehaviour
{
    [Tooltip("The wheel collider of the tyre")]
    [SerializeField] private WheelCollider wheelCollider;

    public void FixedUpdate()
    {
        Tyre();
    }

    public void Tyre()
    {

    }
}
