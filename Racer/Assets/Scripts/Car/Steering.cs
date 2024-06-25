using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SteeringSort
{
    front,
    rear,
    both
}

public class Steering : MonoBehaviour
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

    [Header("Steering")]
    [Tooltip("The type of steering")]
    [SerializeField] private SteeringSort steeringSort = SteeringSort.front;
    [Tooltip("The amount of steering in percentage on the front vs the rear when chosen to use both axles to steer"), Range(100,0)]
    [SerializeField] private float steeringPercentage = 75;
    [Tooltip("The maximum amount at witch you can steer the wheels"), Range(0, 75)]
    [SerializeField] private float steeringDegrees = 30;
    [Tooltip("The amount of steering that can be done at a certain amount of input given"), Curve(0, 0, 1f, 1f, true)]
    [SerializeField] private AnimationCurve steeringCurve;

    //private variables
    private float steeringAxis;

    #endregion

    #region update and input

    public void Update()
    {
        if (steeringSort == SteeringSort.front)
        {
            SteeringFront();
        }
        else if (steeringSort == SteeringSort.rear)
        {
            SteeringRear();
        }
        else if (steeringSort == SteeringSort.both)
        {
            SteeringBoth();
        }
    }

    public void SteeringInput(InputAction.CallbackContext context)
    {
        steeringAxis = context.ReadValue<float>();
    }

    #endregion

    #region steering front

    private void SteeringFront()
    {
        float steeringAmount = 0f;

        if (steeringAxis > 0)
        {
            steeringAmount = steeringCurve.Evaluate(steeringAxis) * steeringDegrees;
        }
        else
        {
            steeringAmount = -steeringCurve.Evaluate(-steeringAxis) * steeringDegrees;
        }

        FL.steerAngle = steeringAmount;
        FR.steerAngle = steeringAmount;
    }

    #endregion

    #region steering rear

    private void SteeringRear()
    {
        float steeringAmount = 0f;

        if (steeringAxis > 0)
        {
            steeringAmount = steeringCurve.Evaluate(steeringAxis) * steeringDegrees;
        }
        else
        {
            steeringAmount = -steeringCurve.Evaluate(-steeringAxis) * steeringDegrees;
        }

        RL.steerAngle = steeringAmount;
        RR.steerAngle = steeringAmount;
    }

    #endregion

    #region steering both

    private void SteeringBoth()
    {
        float frontSteering = steeringPercentage / 100;
        float rearSteering = (100 - steeringPercentage) / 100;

        float steeringAmount = 0f;

        if (steeringAxis > 0)
        {
            steeringAmount = steeringCurve.Evaluate(steeringAxis) * steeringDegrees;
        }
        else
        {
            steeringAmount = -steeringCurve.Evaluate(-steeringAxis) * steeringDegrees;
        }

        FL.steerAngle = steeringAmount * frontSteering;
        FR.steerAngle = steeringAmount * frontSteering;

        RL.steerAngle = steeringAmount * rearSteering;
        RR.steerAngle = steeringAmount * rearSteering;
    }

    #endregion
}
