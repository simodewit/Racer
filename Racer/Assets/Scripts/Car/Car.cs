using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Steering")]
    [Tooltip("The amount of speed at witch you steer the car"), Range(0, 3)]
    [SerializeField] private float steeringSpeed = 0.1f;
    [Tooltip("The maximum amount at witch you can steer the wheels"), Range(0, 75)]
    [SerializeField] private float steeringDegrees = 30;

    [Header("Brakes")]
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float brakeTorque = 200;
    [Tooltip("The amount of brakes to the front or back of the car"), Range(100, 0)]
    [SerializeField] private float brakeBalance = 55;
    [Tooltip("The total amount of force from the pedal that is used"), Range(0, 1)]
    [SerializeField] private float brakeForce = 1;
    [Tooltip("The deadzone in the pedal before the brake is used"), Range(0, 1)]
    [SerializeField] private float brakeDeadzone = 0.1f;

    [Header("Power")]
    [Tooltip("The way that power is transmitted to the wheels. AWD = all wheel drive, RWD = rear wheel drive, FWD = front wheel drive")]
    [SerializeField] private PowerDeliviry powerDeliviry = PowerDeliviry.AWD;
    [Tooltip("The deadzone in the pedal before the throttle is used"), Range(0, 1)]
    [SerializeField] private float throttleDeadzone = 0.1f;
    [Tooltip("The minimum rpm that the engine can run")]
    [SerializeField] private int minRPM = 1000;
    [Tooltip("The maximum rpm that the engine can run")]
    [SerializeField] private int maxRPM = 5000;
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float engineTorque = 200;
    [Tooltip("The final gear ratio of the car")]
    [SerializeField] private float finalGearRatio;
    [Tooltip("The torque given at specific moments"), Curve(0, 0, 1f, 1f, true)]
    [SerializeField] private AnimationCurve torqueCurve;
    [Tooltip("The info for every gear")]
    [SerializeField] private GearInfo[] gears;

    [Header("Code refrences")]
    public int currentRPM;

    private List<WheelCollider> wheelColliders = new List<WheelCollider>();
    private int currentGear;
    private float steeringAmount;

    //all the input axisses
    private float steeringAxis;
    private float throttleAxis;
    private float brakeAxis;

    #endregion

    #region start and update

    public void Start()
    {
        currentGear = 1;

        if (powerDeliviry == PowerDeliviry.AWD)
        {
            wheelColliders.Add(FL);
            wheelColliders.Add(FR);
            wheelColliders.Add(RL);
            wheelColliders.Add(RR);
        }
        else if (powerDeliviry == PowerDeliviry.RWD)
        {
            wheelColliders.Add(RL);
            wheelColliders.Add(RR);
        }
        else if (powerDeliviry == PowerDeliviry.FWD)
        {
            wheelColliders.Add(FL);
            wheelColliders.Add(FR);
        }
    }

    public void FixedUpdate()
    {
        Turning();
        CalculateRPM();
        DriveTrain();
        Braking();
    }

    #endregion

    #region input

    public void SteeringInput(InputAction.CallbackContext context)
    {
        steeringAxis = context.ReadValue<float>();
    }

    public void BrakeInput(InputAction.CallbackContext context)
    {
        brakeAxis = context.ReadValue<float>();
    }

    public void ThrottleInput(InputAction.CallbackContext context)
    {
        throttleAxis = context.ReadValue<float>();
    }

    public void ShiftUp(InputAction.CallbackContext context)
    {
        if (context.performed && currentGear < gears.Length - 1)
        {
            currentGear += 1;
        }
    }

    public void ShiftDown(InputAction.CallbackContext context)
    {
        if (context.performed && currentGear > 0)
        {
            currentGear -= 1;
        }
    }

    #endregion

    #region turning

    public void Turning()
    {
        float turnTowards = steeringDegrees * steeringAxis;

        steeringAmount = Mathf.Lerp(steeringAmount, turnTowards, steeringSpeed * Time.deltaTime);

        //this is in place for a bug
        if (steeringAmount < 0.01 && steeringAmount > -0.01)
        {
            steeringAmount = 0;
        }

        steeringAmount = Mathf.Clamp(steeringAmount, -steeringDegrees, steeringDegrees);

        FL.steerAngle = steeringAmount;
        FR.steerAngle = steeringAmount;
    }

    #endregion

    #region RPM's

    public void CalculateRPM()
    {
        float averageRPM = CalculateWheelRPM();
        float rpmAfterDif = Differential(averageRPM);
        float rpmAfterGearBox = GearBox(rpmAfterDif);

        int newRPM = (int)rpmAfterGearBox;

        if (newRPM < minRPM)
        {
            newRPM = minRPM;
        }

        currentRPM = newRPM;
    }

    public float CalculateWheelRPM()
    {
        //calculate the average rpm's from the wheels attached to the engine
        float addedRPM = 0;

        foreach (var wheel in wheelColliders)
        {
            addedRPM += wheel.rpm;
        }

        float averageRPM = addedRPM / wheelColliders.Count;
        return averageRPM;
    }

    #endregion

    #region gearbox and differential

    public float GearBox(float torque)
    {
        float returnValue = torque * gears[currentGear].gearRatio;

        return returnValue;
    }

    public float Differential(float torque)
    {
        float returnValue = torque * finalGearRatio;

        return returnValue;
    }

    #endregion

    #region driveTrain

    public void DriveTrain()
    {
        //the torque curve
        float placeInCurve = currentRPM / maxRPM;
        float curveMultiplier = torqueCurve.Evaluate(placeInCurve);
        float torqueOutput = engineTorque * curveMultiplier;

        //the gearbox and dif
        float torqueAfterGearbox = GearBox(torqueOutput);
        float torqueAfterDif = Differential(torqueAfterGearbox);

        float outputToWheels = 0;

        if (throttleAxis > throttleDeadzone)
        {
            outputToWheels = torqueAfterDif * throttleAxis;
        }

        if (currentRPM >= maxRPM)
        {
            outputToWheels = 0;
        }

        //the power deliviry
        if (powerDeliviry == PowerDeliviry.AWD)
        {
            float torquePerWheel = outputToWheels / 4;
            float outputTorque = torquePerWheel / RL.radius;
            AllWheelDrive(outputTorque);
        }
        else if(powerDeliviry == PowerDeliviry.RWD)
        {
            float torquePerWheel = outputToWheels / 2;
            float outputTorque = torquePerWheel / RL.radius;
            RearWheelDrive(outputTorque);
        }
        else if (powerDeliviry == PowerDeliviry.FWD)
        {
            float torquePerWheel = outputToWheels / 2;
            float outputTorque = torquePerWheel / FL.radius;
            FrontWheelDrive(outputTorque);
        }
    }

    #endregion

    #region AllWheelDrive

    public void AllWheelDrive(float torque)
    {
        FL.motorTorque = torque;
        FR.motorTorque = torque;
        RL.motorTorque = torque;
        RR.motorTorque = torque;
    }

    #endregion

    #region RearWheelDrive

    public void RearWheelDrive(float torque)
    {
        RL.motorTorque = torque;
        RR.motorTorque = torque;
    }

    #endregion

    #region FrontWheelDrive

    public void FrontWheelDrive(float torque)
    {
        FL.motorTorque = torque;
        FR.motorTorque = torque;
    }

    #endregion

    #region braking

    public void Braking()
    {
        if (brakeAxis > brakeDeadzone)
        {
            float frontBrakes = brakeBalance / 100;
            float rearBrakes = (100 - brakeBalance) / 100;

            float wheelTorque = brakeTorque * 0.5f;

            FL.brakeTorque = wheelTorque * frontBrakes * brakeAxis * brakeForce;
            FR.brakeTorque = wheelTorque * frontBrakes * brakeAxis * brakeForce;
            RL.brakeTorque = wheelTorque * rearBrakes * brakeAxis * brakeForce;
            RR.brakeTorque = wheelTorque * rearBrakes * brakeAxis * brakeForce;
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
    [Tooltip("The number of the gear that you are in. (reverse = -1, neutral = 0, 1st gear = 1, etc)")]
    public int gearNumber;
    [Tooltip("The amount that the engine torque should be multiplied")]
    public float gearRatio;
}
