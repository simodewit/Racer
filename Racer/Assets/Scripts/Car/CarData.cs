using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour
{
    [Header("refrences")]
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private UnrealisticEngine engine;
    [SerializeField] private GearBox gearBox;
    [SerializeField] private Brakes brakes;
    [SerializeField] private FuelTank fuelTank;

    [Header("Front right")]
    [SerializeField] private Tyre tyreFR;
    [SerializeField] private Suspension suspensionFR;

    [Header("Front left")]
    [SerializeField] private Tyre tyreFL;
    [SerializeField] private Suspension suspensionFL;

    [Header("Rear right")]
    [SerializeField] private Tyre tyreRR;
    [SerializeField] private Suspension suspensionRR;

    [Header("Rear left")]
    [SerializeField] private Tyre tyreRL;
    [SerializeField] private Suspension suspensionRL;

    [Space(50)]
    [Header("These are all read values")]
    [Header("Output")]
    public int gear;
    public float rpm;
    public float speed;
    public float engineTorque;
    public float brakeTorque;
    public float fuel;

    [Header("Front right")]
    public float suspensionDistanceFR;
    public float gripFR;

    [Header("Front left")]
    public float suspensionDistanceLF;
    public float gripFL;

    [Header("Rear right")]
    public float suspensionDistanceRR;
    public float gripRR;

    [Header("Rear left")]
    public float suspensionDistanceRL;
    public float gripRL;

    public void Update()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        gear = gearBox.currentGear;
        rpm = engine.rpm;
        speed = carRb.velocity.magnitude * 3.6f;
        engineTorque = engine.outputTorque;
        brakeTorque = brakes.brakeOutput;
        fuel = fuelTank.fuel;

        suspensionDistanceFR = suspensionFR.distanceInSpring;
        suspensionDistanceLF = suspensionFL.distanceInSpring;
        suspensionDistanceRR = suspensionRR.distanceInSpring;
        suspensionDistanceRL = suspensionRL.distanceInSpring;

        gripFR = tyreFR.totalSidewayGrip;
        gripFL = tyreFL.totalSidewayGrip;
        gripRR = tyreRR.totalSidewayGrip;
        gripRL = tyreRL.totalSidewayGrip;
    }
}
