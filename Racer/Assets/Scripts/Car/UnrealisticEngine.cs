using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnrealisticEngine : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private GearBox gearBox;
    [SerializeField] private Rigidbody rb;

    [Header("Power variables")]
    [Tooltip("The maximum rpm that the engine can run"), Range(0, 25000)]
    public int maxRPM = 5000; //has to be public for the gearbox
    [Tooltip("The minimum rpm that the car can have")]
    [SerializeField] private float minRPM = 1000;
    [Tooltip("The amount of torque from the rpm's of the engine"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve torqueCurve;
    [Tooltip("The maximum amount of torque to the wheels")]
    [SerializeField] private float maxTorque;

    [HideInInspector] public float rpm;
    [HideInInspector] public float outputTorque;
    [HideInInspector] public float rpmFactor = 1;

    private float throttleAxis;

    public void Update()
    {
        Power();
    }

    public void ThrottleInput(InputAction.CallbackContext context)
    {
        throttleAxis = context.ReadValue<float>();
    }

    private void Power()
    {
        float factor = rb.velocity.magnitude * 3.6f / gearBox.gears[gearBox.currentGear + 1].maxSpeed;
        rpm = maxRPM * factor;

        float torque = torqueCurve.Evaluate(factor) * maxTorque * rpmFactor * throttleAxis;

        if (rpm < minRPM)
        {
            rpm = minRPM;
        }

        if (rpm > maxRPM)
        {
            rpm = maxRPM;
            torque = 0;
        }

        outputTorque = torque;
    }
}
