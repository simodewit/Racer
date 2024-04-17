using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum PowerDeliviry
{
    AWD,
    RWD,
    FWD
}

public enum GearType
{
    Drive,
    Neutral,
    Reverse
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
    [Tooltip("The info for every gear")]
    [SerializeField] private GearInfo[] gearInfo;
    [Tooltip("The lowest amount of rpm for of the car")]
    [SerializeField] private float minRPM;
    [Tooltip("The highest amount of rpm for of the car")]
    [SerializeField] private float maxRPM;

    [Header("Brakes")]
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float brakeTorque = 200;
    [Tooltip("The amount of brakes to the front or back of the car"), Range(0, 100)]
    [SerializeField] private float brakeBalance = 55;

    [Header("Steering")]
    [Tooltip("The amount of speed at witch you steer the car"), Range(0, 3)]
    [SerializeField] private float steeringSpeed = 1;
    [Tooltip("The maximum amount at witch you can steer the wheels"), Range(0, 75)]
    [SerializeField] private float totalSteering = 40;
    [Tooltip("The deadzone of the steering when the wheels start centering back"), Range(0, 3)]
    [SerializeField] private float steeringDeadzone = 0.5f;
    [Tooltip("The speed at witch the steering snaps back towards the middle"), Range(0, 0.1f)]
    [SerializeField] private float snapBackSpeed = 0.02f;

    private float steeringAmount;
    private int currentGear;

    #endregion

    #region update

    public void Update()
    {
        Turning();
        AllWheelDrive();
        RearWheelDrive();
        FrontWheelDrive();
        Braking();
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

    #region AllWheelDrive

    public void AllWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.AWD)
        {
            return;
        }

        float axis = Input.GetAxis("Vertical");

        if (axis > 0)
        {
            float torquePerWheel = CalculateTorque(axis) * 0.25f;

            FL.motorTorque = torquePerWheel;
            FR.motorTorque = torquePerWheel;
            RL.motorTorque = torquePerWheel;
            RR.motorTorque = torquePerWheel;
        }
    }

    #endregion

    #region RearWheelDrive

    public void RearWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.RWD)
        {
            return;
        }

        float axis = Input.GetAxis("Vertical");

        if (axis > 0)
        {
            float torquePerWheel = engineTorque * 0.5f;

            RL.motorTorque = torquePerWheel * axis;
            RR.motorTorque = torquePerWheel * axis;
        }
    }

    #endregion

    #region FrontWheelDrive

    public void FrontWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.FWD)
        {
            return;
        }

        float axis = Input.GetAxis("Vertical");

        if (axis > 0)
        {
            float torquePerWheel = engineTorque * 0.5f;

            FL.motorTorque = torquePerWheel * axis;
            FR.motorTorque = torquePerWheel * axis;
        }
    }

    #endregion

    #region gears

    private float CalculateTorque(float axis)
    {
        // Get the gear ratio from another class with more info
        float gearRatio = gearInfo[currentGear].gearRatio;

        // Calculate engine RPM based on the front left wheel RPM
        float engineRPM = RL.rpm * gearRatio;

        print(engineRPM);

        // Ensure engine RPM is within the valid range
        float normalizedRPM = Mathf.Clamp01((engineRPM - minRPM) / (maxRPM - minRPM));

        // Define torque curve points
        float lowTorque = 0.2f * engineTorque;
        float highTorque = engineTorque;

        // Interpolate torque based on RPM
        float torque = Mathf.Lerp(lowTorque, highTorque, Mathf.Pow(normalizedRPM, 2));

        torque *= axis;
        return torque;
    }

    #endregion

    #region braking

    public void Braking()
    {
        float axis = Input.GetAxis("Vertical");

        if (axis < 0)
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
    public GearType gearType;
    public float gearRatio;
}
