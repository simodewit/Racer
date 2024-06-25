using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aerodynamics : MonoBehaviour
{
    [SerializeField] private AeroPlaces[] aeroInfo;
    [SerializeField] private Rigidbody rb;
    private WindSpeed windSpeed;

    #region start and update

    public void Start()
    {
        windSpeed = FindAnyObjectByType<WindSpeed>();
    }

    public void FixedUpdate()
    {
        ApplyDownForce();
    }

    #endregion

    #region apply downforce

    public void ApplyDownForce()
    {
        if (windSpeed == null)
        {
            print("Windspeed is missing (from aero script)");
            return;
        }

        //calculate wind speeds
        float dotProduct = Vector3.Dot(rb.transform.forward, windSpeed.transform.forward);

        foreach (var info in aeroInfo)
        {
            //calculate velocity
            float velocity = rb.GetPointVelocity(info.downforcePlace.position).z;
            float squaredVelocity = velocity * velocity;
            float endVelocity = squaredVelocity - dotProduct * windSpeed.windSpeed;

            if (endVelocity < 0)
            {
                endVelocity = 0;
            }

            //calculate and apply downforce
            Vector3 downForce = -transform.up * (info.amountOfForce / 100 * (squaredVelocity / 15));
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