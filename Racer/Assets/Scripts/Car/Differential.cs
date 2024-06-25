using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Differential : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [Tooltip("The refrence to the rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The rear left wheelController")]
    [SerializeField] private Tyre leftWheel;
    [Tooltip("The rear right wheelController")]
    [SerializeField] private Tyre rightWheel;

    [Header("Differential")]
    [Tooltip("The gearbox script")]
    [SerializeField] private GearBox gearBox;
    [Tooltip("The final gear ratio of the car")]
    public float finalGearRatio; // has to be public for the engine script
    [Tooltip("How much of the differential can be opened or closed"), Range(0, 100)]
    [SerializeField] private float diffLock;

    [Header("resistance")]
    [Tooltip("The rolling resistance coefficient of the car (normal asphalt coefficient is 0.02)"), Range(0.01f, 0.1f)]
    [SerializeField] private float rollingCoefficient = 0.02f;

    //get variables
    [HideInInspector] public float rollingResistance;

    //private variables
    private int amountOfDifferentials;

    #endregion

    #region start and update

    public void Start()
    {
        Differential[] diffs = FindObjectsOfType<Differential>();
        amountOfDifferentials = diffs.Length;
    }

    public void FixedUpdate()
    {
        Diff();
    }

    public void Update()
    {
        float weight = carRb.mass * Physics.gravity.y;
        float resistance = rollingCoefficient * weight;
        rollingResistance = resistance / finalGearRatio;
    }

    #endregion

    #region differential

    private void Diff()
    {
        //calculate the amount of torque before dividing
        float entryTorque = gearBox.outputTorque * finalGearRatio / amountOfDifferentials;

        //calculate the percentages
        float average = (leftWheel.rpm + rightWheel.rpm) / 2;
        float leftFactor = leftWheel.rpm / average;
        float rightFactor = rightWheel.rpm / average;

        //calculate the min and max for the clamp
        float min = (100 - diffLock / 2) / 100;
        float max = (100 + diffLock / 2) / 100;

        //clamp the vallues
        leftFactor = Mathf.Clamp(leftFactor, min, max);
        rightFactor = Mathf.Clamp(rightFactor, min, max);

        //temp
        leftWheel.motorTorque = entryTorque / 2 * leftFactor;
        rightWheel.motorTorque = entryTorque / 2 * rightFactor;
    }

    #endregion
}
