using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAero : MonoBehaviour
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
            Vector3 totalDownForce = -transform.up * (info.amountOfForce * speed);
            rb.AddForceAtPosition(totalDownForce, info.downforcePlace.position);
        }
    }

    #endregion
}

[Serializable]

public class AeroPlaces
{
    [Tooltip("The place where the downforce is applied")]
    public Transform downforcePlace;
    [Tooltip("The amount of downforce on this place")]
    public float amountOfForce;
}