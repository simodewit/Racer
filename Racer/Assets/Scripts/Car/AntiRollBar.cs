using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private WheelCollider leftWheel;
    [SerializeField] private WheelCollider rightWheel;
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

        if (rightWheel.isGrounded)
        {
            rb.AddForceAtPosition(rightWheel.transform.up * -antiRollForce, rightWheel.transform.position);
        }
        if(leftWheel.isGrounded)
        {
            rb.AddForceAtPosition(leftWheel.transform.up * antiRollForce, leftWheel.transform.position);
        }
    }

    public float CalculateTravel(WheelCollider wheel)
    {
        //calculate how far the suspension is in its travel
        WheelHit hit;
        float leftDistance = 0;
        float travel = 1;

        if (wheel.isGrounded)
        {
            if (wheel.GetGroundHit(out hit))
            {
                leftDistance = Vector3.Distance(wheel.transform.position, hit.point);
            }

            travel = (leftDistance - wheel.radius) / wheel.suspensionDistance;
        }

        return travel;
    }
}
