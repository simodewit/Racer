using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations;

public enum PowerDeliviry
{
    AWD,
    RWD,
    FWD
}

public class Car : MonoBehaviour
{
    #region variables

    [Header("Wheels")]
    [Tooltip("The front left wheelCollider")]
    [SerializeField] private WheelCollider FL;
    [Tooltip("The front right wheelCollider")]
    [SerializeField] private WheelCollider FR;
    [Tooltip("The rear left wheelCollider")]
    [SerializeField] private WheelCollider RL;
    [Tooltip("The rear right wheelCollider")]
    [SerializeField] private WheelCollider RR;

    [Header("Power")]
    [Tooltip("The way that power is transmitted to the wheels. AWD = all wheel drive, RWD = rear wheel drive, FWD = front wheel drive")]
    [SerializeField] private PowerDeliviry powerDeliviry = PowerDeliviry.AWD;
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float engineTorque = 200;
    [Tooltip("The amount of torque that is used of the engineTorque when in reverse"), Range(0, 1)]
    [SerializeField] private float reverseTorque;
    [Tooltip("The max speed that the car is capable of hitting")]
    [SerializeField] private float topSpeed = 180;
    //[Tooltip("The highest amount of rpm for of the car"), Range(0,20000)]
    //[SerializeField] private float maxRPM = 5000;
    [Tooltip("The torque given at specific moments")]
    [SerializeField] private AnimationCurve torqueCurve;
    [Tooltip("The info for every gear")]
    [SerializeField] private GearInfo[] gearInfo;

    [Header("Brakes")]
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float brakeTorque = 200;
    [Tooltip("The amount of brakes to the front or back of the car"), Range(0, 100)]
    [SerializeField] private float brakeBalance = 55;
    [Tooltip("The deadzone in the pedal before the throttle or brake is used")]
    [SerializeField] private float pedalDeadzone;

    [Header("Steering")]
    [Tooltip("The amount of speed at witch you steer the car"), Range(0, 3)]
    [SerializeField] private float steeringSpeed = 1;
    [Tooltip("The maximum amount at witch you can steer the wheels"), Range(0, 75)]
    [SerializeField] private float totalSteering = 40;
    [Tooltip("The deadzone of the steering when the wheels start centering back"), Range(0, 3)]
    [SerializeField] private float steeringDeadzone = 0.5f;
    [Tooltip("The speed at witch the steering snaps back towards the middle"), Range(0, 0.1f)]
    [SerializeField] private float snapBackSpeed = 0.02f;

    private Rigidbody rb;
    private int currentGear;
    private float steeringAmount;
    private float backwardTorque;
    private float speedInKph;

    #endregion

    #region start and update

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        backwardTorque = -engineTorque * reverseTorque;
    }

    public void Update()
    {
        Turning();
        Speedometer();
        GearShifts();
        AllWheelDrive();
        Braking();
    }

    #endregion

    #region speedometer

    public void Speedometer()
    {
        speedInKph = rb.velocity.magnitude * 3.6f;
    }

    #endregion

    #region turning

    public void Turning()
    {
        //temporary turning code. should be changed to the new input system soon
        float turningAxis = Input.GetAxis("Horizontal");

        float totalTurning = turningAxis * steeringSpeed;

        if (totalTurning > -steeringDeadzone && totalTurning < steeringDeadzone)
        {
            steeringAmount = Mathf.Lerp(steeringAmount, 0, snapBackSpeed);
        }
        else
        {
            steeringAmount += totalTurning;
        }

        steeringAmount = Mathf.Clamp(steeringAmount, -totalSteering, totalSteering);

        FL.steerAngle = steeringAmount;
        FR.steerAngle = steeringAmount;
    }

    #endregion

    #region gear shifts

    public void GearShifts()
    {
        //temporary shifting code. should be changed to the new input system soon
        if (Input.GetKeyDown(KeyCode.E) && currentGear < gearInfo.Length - 1)
        {
            currentGear += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && currentGear > 0)
        {
            currentGear -= 1;
        }
    }

    #endregion

    #region AllWheelDrive

    public void AllWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.AWD)
        {
            return;
        }

        float axis = Input.GetAxis("Vertical");

        if (axis > pedalDeadzone)
        {
            float wheelTorque = CalculateTotalTorque(gearInfo[currentGear]) * 0.25f;

            FL.motorTorque = wheelTorque;
            FR.motorTorque = wheelTorque;
            RL.motorTorque = wheelTorque;
            RR.motorTorque = wheelTorque;
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;
            RL.motorTorque = 0;
            RR.motorTorque = 0;
        }
    }

    #endregion

    #region torque

    public float CalculateTotalTorque(GearInfo gear)
    {
        float totalTorque;

        if (gear.gearNumber != -1)
        {
            float placeInCurve = (speedInKph / 2) / (topSpeed / 2);
            float torqueMultiplier = torqueCurve.Evaluate(placeInCurve);

            if (speedInKph >= gear.maxSpeed)
            {
                torqueMultiplier = 0;
            }

            totalTorque = engineTorque * torqueMultiplier;
        }
        else
        {
            float torque = backwardTorque;

            if (speedInKph <= gear.maxSpeed)
            {
                torque = 0;
            }

            totalTorque = torque;
        }

        return totalTorque;
    }

    #endregion

    #region braking

    public void Braking()
    {
        //temporary braking code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        if (axis < pedalDeadzone)
        {
            float frontBrakes = brakeBalance / 100;
            float rearBrakes = (100 - brakeBalance) / 100;

            float wheelTorque = brakeTorque * 0.5f;

            FL.brakeTorque = wheelTorque * frontBrakes * -axis;
            FR.brakeTorque = wheelTorque * frontBrakes * -axis;
            RL.brakeTorque = wheelTorque * rearBrakes * -axis;
            RR.brakeTorque = wheelTorque * rearBrakes * -axis;
        }
        else
        {
            FL.brakeTorque = 0;
            FR.brakeTorque = 0;
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }
    }

    #endregion
}

[Serializable]
public class GearInfo
{
    public int gearNumber;
    public float maxSpeed;
}
