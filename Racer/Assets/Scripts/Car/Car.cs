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
    [Tooltip("The amount of torque that is used of the engineTorque when in reverse"), Range(0, 1)]
    [SerializeField] private float reverseTorque = 0.1f;
    [Tooltip("The deadzone in the pedal before the throttle is used"), Range(0, 1)]
    [SerializeField] private float throttleDeadzone = 0.1f;
    [Tooltip("The maximum rpm that the engine can have")]
    [SerializeField] private int maxRPM = 5000;
    [Tooltip("The torque given at specific moments")]
    [SerializeField] private AnimationCurve torqueCurve;
    [Tooltip("The info for every gear")]
    [SerializeField] private GearInfo[] gears;

    private Rigidbody rb;
    private int currentGear;
    private int currentRPM;
    private float steeringAmount;
    private float backwardTorque;
    private List<WheelCollider> wheelColliders = new List<WheelCollider>();

    #endregion

    #region start and update

    public void Start()
    {
        backwardTorque = -gears[currentGear].engineTorque * reverseTorque;
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
        RPM();
        GearShifts();
        AllWheelDrive();
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

    #region gear shifts

    public void GearShifts()
    {
        //temporary shifting code. should be changed to the new input system soon
        if (Input.GetKeyDown(KeyCode.E) && currentGear < gears.Length - 1)
        {
            currentGear += 1;

            if (currentGear == 0)
            {
                currentRPM = (int)(currentRPM * (gears[currentGear].rpmAddMultiplier + 1));
            }
            else
            {
                currentRPM = (int)(currentRPM * gears[currentGear].rpmLossMultiplier);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q) && currentGear > 0)
        {
            currentGear -= 1;

            if (currentGear == -1)
            {
                currentRPM = (int)(currentRPM * gears[currentGear].rpmLossMultiplier);
            }
            else
            {
                currentRPM = (int)(currentRPM * (gears[currentGear].rpmAddMultiplier + 1));
            }
        }
    }

    #endregion

    #region RPM's

    public void RPM()
    {
        //temporary throttle code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        //calculate the rpm's to add
        int rpmToAdd = 0;

        if (axis > throttleDeadzone)
        {
            rpmToAdd = (int)CalculateRPM();

            if (currentRPM > maxRPM)
            {
                rpmToAdd = 0;
            }
        }

        ////calculate the rpm's to remove
        //float currentSpeed = CalculateSpeedFromWheel();
        //float gearTopSpeed = (gears[currentGear].engineTorque / rb.drag) / gears[currentGear].gearRatio;

        //float targetRPM = maxRPM * (currentSpeed / (gearTopSpeed * 3.6f));
        //int rpmToRemove = currentRPM - (int)targetRPM;

        //add or remove rpm's from the current rpm's
        currentRPM += rpmToAdd;
        print(currentRPM);
    }

    public float CalculateSpeedFromWheel()
    {
        //calculate the average rpm's from the wheels attached to the engine
        WheelHit hit;
        float totalRPM = 0;

        foreach (var wheel in wheelColliders)
        {
            if (wheel.GetGroundHit(out hit))
            {
                totalRPM += wheel.rpm * (wheel.radius * 2 * Mathf.PI) / 60;
            }
        }

        float averageRPM = totalRPM / wheelColliders.Count;
        averageRPM = averageRPM * 3.6f;

        return averageRPM;
    }

    public float CalculateRPM()
    {
        //determine how much throttle the player uses
        float axis = Input.GetAxis("Vertical");
        float throttleInput = 0;

        if (axis > throttleDeadzone)
        {
            throttleInput = axis;
        }


        //get the extra rpm's that should be given from losing grip
        float totalGripLoss = 0;

        foreach (var wheel in wheelColliders)
        {
            WheelHit hit;

            if (wheel.GetGroundHit(out hit))
            {
                totalGripLoss += Mathf.Abs(hit.sidewaysSlip);
            }
        }

        float gripLossPerWheel = totalGripLoss / wheelColliders.Count;
        gripLossPerWheel += 1;

        //the output
        float totalOutput = throttleInput * gripLossPerWheel * gears[currentGear].rpmMultiplier;
        return totalOutput;
    }

    #endregion

    #region torque

    public float CalculateTotalTorque(GearInfo gear)
    {
        float totalTorque;

        if (gear.gearNumber != -1)
        {
            float placeInCurve = (currentRPM / 2) / (maxRPM / 2);
            float torqueMultiplier = torqueCurve.Evaluate(placeInCurve);

            totalTorque = gears[currentGear].engineTorque * gear.gearRatio * torqueMultiplier;
        }
        else
        {
            float placeInCurve = (currentRPM / 2) / (maxRPM / 2);
            float torqueMultiplier = torqueCurve.Evaluate(placeInCurve);

            totalTorque = backwardTorque * gear.gearRatio * torqueMultiplier;
        }

        return totalTorque;
    }

    #endregion

    #region AllWheelDrive

    public void AllWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.AWD)
        {
            return;
        }

        //temporary throttle code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        if (axis > throttleDeadzone)
        {
            float wheelTorque = CalculateTotalTorque(gears[currentGear]) * 0.25f;

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

    #region RearWheelDrive

    public void RearWheelDrive()
    {
        if (powerDeliviry != PowerDeliviry.RWD)
        {
            return;
        }

        //temporary throttle code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        if (axis > throttleDeadzone)
        {
            float wheelTorque = CalculateTotalTorque(gears[currentGear]) * 0.5f;

            RL.motorTorque = wheelTorque;
            RR.motorTorque = wheelTorque;
        }
        else
        {
            RL.motorTorque = 0;
            RR.motorTorque = 0;
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

        //temporary throttle code. should be changed to the new input system soon
        float axis = Input.GetAxis("Vertical");

        if (axis > throttleDeadzone)
        {
            float wheelTorque = CalculateTotalTorque(gears[currentGear]) * 0.5f;

            FL.motorTorque = wheelTorque;
            FR.motorTorque = wheelTorque;
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;
        }
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
    [Tooltip("The speed at wich you get rpm's in this gear")]
    public float rpmMultiplier;
    [Tooltip("The amount of rpm's you have left from shifting into this gear"), Range(0, 1)]
    public float rpmLossMultiplier;
    [Tooltip("The amount of rpm's you get from shifting down into this gear"), Range(0, 1)]
    public float rpmAddMultiplier;
    [Tooltip("The amount that the engine torque should be multiplied")]
    public float gearRatio;
    [Tooltip("The total newton meters of torque that the car has in this gear"), Range(0, 5000)]
    public float engineTorque = 200;
}
