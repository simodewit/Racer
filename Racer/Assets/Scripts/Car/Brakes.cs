using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Brakes : MonoBehaviour
{
    #region variables

    [Header("Wheels")]
    [Tooltip("The front left wheelController")]
    [SerializeField] private Tyre FL;
    [Tooltip("The front right wheelController")]
    [SerializeField] private Tyre FR;
    [Tooltip("The rear left wheelController")]
    [SerializeField] private Tyre RL;
    [Tooltip("The rear right wheelController")]
    [SerializeField] private Tyre RR;

    [Header("Brakes")]
    [Tooltip("The total newton meters of torque that the car has"), Range(0, 5000)]
    [SerializeField] private float brakeTorque = 200;
    [Tooltip("The amount of brakes to the front or back of the car"), Range(100, 0)]
    [SerializeField] private float brakeBalance = 55;
    [Tooltip("The max amount of percentage that the brake pedal alowes"), Range(0, 100)]
    [SerializeField] private float maxForce = 100;
    [Tooltip("The deadzone in the pedal before the brake is used in percentage"), Range(0, 100)]
    [SerializeField] private float brakeDeadzone = 0;

    [HideInInspector] public float brakeOutput;

    //private variables
    private float brakeAxis;

    #endregion

    #region update and input

    public void Update()
    {
        Braking();
        brakeOutput = brakeTorque * brakeAxis;
    }

    public void BrakeInput(InputAction.CallbackContext context)
    {
        brakeAxis = context.ReadValue<float>();
    }

    #endregion

    #region brakes

    private void Braking()
    {
        if (brakeAxis > (brakeDeadzone / 100) && brakeAxis <= (maxForce / 100))
        {
            float frontBrakes = brakeBalance / 100;
            float rearBrakes = (100 - brakeBalance) / 100;

            float wheelTorque = brakeTorque * 0.5f;

            FL.brakeTorque = wheelTorque * frontBrakes * brakeAxis;
            FR.brakeTorque = wheelTorque * frontBrakes * brakeAxis;
            RL.brakeTorque = wheelTorque * rearBrakes * brakeAxis;
            RR.brakeTorque = wheelTorque * rearBrakes * brakeAxis;
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
