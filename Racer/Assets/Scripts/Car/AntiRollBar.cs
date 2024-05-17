using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private WheelController leftWheel;
    [SerializeField] private WheelController rightWheel;
    [SerializeField] private float stiffness;

    public void FixedUpdate()
    {
        AntiRoll();
    }

    public void AntiRoll()
    {
        float rightTravel = CalculateTravel(rightWheel);
        float leftTravel = CalculateTravel(leftWheel);

        float antiRollForce = (rightTravel - leftTravel) * stiffness;

        carRb.AddForceAtPosition(rightWheel.transform.up * -antiRollForce, rightWheel.transform.position);
        carRb.AddForceAtPosition(leftWheel.transform.up * antiRollForce, leftWheel.transform.position);
    }

    public float CalculateTravel(WheelController wheel)
    {
        //calculate how much body roll there is
        float travel = wheel.targetPos.localPosition.y - wheel.transform.localPosition.y; ;
        return travel;
    }
}
