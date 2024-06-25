using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Suspension leftSuspension;
    [SerializeField] private Suspension rightSuspension;
    [SerializeField] private Tyre leftTyre;
    [SerializeField] private Tyre rightTyre;

    [Header("Settings")]
    [SerializeField] private float stiffness;

    #endregion

    #region update

    public void FixedUpdate()
    {
        AntiRoll();
    }

    #endregion

    #region anti roll bar

    private void AntiRoll()
    {
        //check if the car is grounded
        if (!leftTyre.isGrounded || !rightTyre.isGrounded)
        {
            return;
        }

        //calculate the travel of the springs
        float rightTravel = CalculateTravel(rightSuspension);
        float leftTravel = CalculateTravel(leftSuspension);

        //calculate the force that the ARB should give to the suspension
        float antiRollForce = (rightTravel - leftTravel) * stiffness;

        //add the force to the suspension
        carRb.AddForceAtPosition(rightSuspension.transform.up * -antiRollForce, rightSuspension.transform.position);
        carRb.AddForceAtPosition(leftSuspension.transform.up * antiRollForce, leftSuspension.transform.position);
    }

    private float CalculateTravel(Suspension suspension)
    {
        //calculate the spring travel
        float travel = suspension.springTargetPos.localPosition.y - suspension.transform.localPosition.y;
        return travel;
    }

    #endregion
}
