using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
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

    [Header("Steering")]
    [Tooltip("The amount of speed at witch you steer the car"), Range(0, 3)]
    [SerializeField] private float steeringSpeed = 0.1f;
    [Tooltip("The maximum amount at witch you can steer the wheels"), Range(0, 75)]
    [SerializeField] private float totalSteering = 30;
    [Tooltip("The speed at witch the steering snaps back towards the middle"), Range(0, 0.1f)]
    [SerializeField] private float snapBackSpeed = 0.02f;

    [Header("Brakes")]
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float brakeTorque = 200;
    [Tooltip("The amount of brakes to the front or back of the car"), Range(0, 100)]
    [SerializeField] private float brakeBalance = 55;
    [Tooltip("The deadzone in the pedal before the brake is used"), Range(0, 1)]
    [SerializeField] private float brakeDeadzone = 0.1f;

    [Header("Power")]
    [Tooltip("The way that power is transmitted to the wheels. AWD = all wheel drive, RWD = rear wheel drive, FWD = front wheel drive")]
    [SerializeField] private PowerDeliviry powerDeliviry = PowerDeliviry.AWD;
    [Tooltip("The deadzone in the pedal before the throttle is used"), Range(0, 1)]
    [SerializeField] private float throttleDeadzone = 0.1f;
    [Tooltip("The maximum rpm that the engine can have")]
    [SerializeField] private int maxRPM = 5000;
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float engineTorque = 200;
    [Tooltip("The final gear ratio of the car")]
    [SerializeField] private float finalGearRatio;
    [Tooltip("The torque given at specific moments")]
    [SerializeField] private AnimationCurve torqueCurve;
    [Tooltip("The info for every gear")]
    [SerializeField] private GearInfo[] gears;

    [Header("Code refrences")]
    public int currentRPM;

    private Rigidbody rb;
    private List<WheelCollider> wheelColliders = new List<WheelCollider>();
    private int currentGear;
    private float steeringAmount;

    #endregion

    #region start and update

    public void Start()
    {
        currentGear = 1;
        rb = GetComponent<Rigidbody>();

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

    public void Update()
    {
        Turning();
        GearShifts();
        CalculateRPM();
        DriveTrain();
        Braking();
    }

    #endregion

    #region turning

    public void Turning()
    {
        //temporary turning code. should be changed to the new input system soon
        float turningAxis = Input.GetAxis("Horizontal");
        bool isTurning = false;

        //this is used to prevent delay
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isTurning = true;
        }

        float totalTurning = turningAxis * steeringSpeed;

        if (isTurning == false)
        {
            steeringAmount = Mathf.Lerp(steeringAmount, 0, snapBackSpeed);
        }
        else
        {
            steeringAmount += totalTurning;
        }

        //this is in place for a bug
        if (steeringAmount < 0.01 && steeringAmount > -0.01)
        {
            steeringAmount = 0;
        }

        steeringAmount = Mathf.Clamp(steeringAmount, -totalSteering, totalSteering);

        FL.steerAngle = steeringAmount;
        FR.steerAngle = steeringAmount;
    }

    #endregion

    #region RPM's

    public void CalculateRPM()
    {
        float averageRPM = CalculateWheelRPM();
        float rpmAfterDif = ReverseDifferential(averageRPM);
        float rpmAfterGearBox = ReverseDifferential(rpmAfterDif);

        int newRPM = (int)(rpmAfterGearBox);

        if (newRPM > maxRPM)
        {
            newRPM = maxRPM;
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

    #region gear shifts

    public void GearShifts()
    {
        //temporary shifting code. should be changed to the new input system soon
        if (Input.GetKeyDown(KeyCode.E) && currentGear < gears.Length - 1)
        {
            currentGear += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && currentGear > 0)
        {
            currentGear -= 1;
        }
    }

    #endregion

    #region gearbox and differential

    public float GearBox(float torque)
    {
        float returnValue = torque * gears[currentGear].gearRatio;

        return returnValue;
    }

    public float ReverseGearbox(float rpm)
    {
        float returnValue = rpm / gears[currentGear].gearRatio;

        return returnValue;
    }

    public float Differential(float torque)
    {
        float returnValue = torque * finalGearRatio;

        return returnValue;
    }

    public float ReverseDifferential(float rpm)
    {
        float returnValue = rpm / finalGearRatio;

        return returnValue;
    }

    #endregion

    #region driveTrain

    public void DriveTrain()
    {
        //the torque curve
        float placeInCurve = (currentRPM * 100) / maxRPM;
        float curveMultiplier = torqueCurve.Evaluate(placeInCurve);
        float torqueOutput = engineTorque * curveMultiplier;

        //the gearbox and dif
        float torqueAfterGearbox = GearBox(torqueOutput);
        float torqueAfterDif = Differential(torqueAfterGearbox);

        //the input
        float axis = Input.GetAxis("Vertical");
        float outputToWheels = 0;

        if (axis > throttleDeadzone)
        {
            outputToWheels = torqueAfterDif * axis;
        }

        print(outputToWheels);

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
        //temporary braking code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        if (axis < brakeDeadzone)
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
    [Tooltip("The number of the gear that you are in. (reverse = -1, neutral = 0, 1st gear = 1, etc)")]
    public int gearNumber;
    [Tooltip("The amount that the engine torque should be multiplied")]
    public float gearRatio;
}
