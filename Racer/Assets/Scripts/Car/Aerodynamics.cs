using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aerodynamics : MonoBehaviour
{
    [SerializeField] private AeroPlaces[] aeroInfo;
    private Rigidbody rb;

    #region start and update

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        ApplyDownForce();
    }

    #endregion

    #region apply downforce

    public void ApplyDownForce()
    {
        float speed = rb.velocity.sqrMagnitude;

        foreach (var info in aeroInfo)
        {
            Vector3 downForce = -transform.up * (info.amountOfForce / 100 * (speed / 15));
            rb.AddForceAtPosition(downForce, info.downforcePlace.position);
        }
    }



    #endregion
}

[Serializable]

public class AeroPlaces
{
    [Tooltip("The place where the downforce is applied")]
    public Transform downforcePlace;
    [Tooltip("The amount of downforce on this place at 100kph")]
    public float amountOfForce;
}