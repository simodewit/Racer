using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tyre : MonoBehaviour
{
    #region variables

    [Header("General info")]
    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody rb;
    [Tooltip("The wheelCollider of this tyre")]
    [SerializeField] private WheelCollider wheelCollider;
    //[Tooltip("The tyre compound that is fitted onto the car")]
    //[SerializeField] private TyreCompound compound;
    [Tooltip("The air temperature of the location of the circuit in degrees celsius")]
    [SerializeField] private float airTemperature;
    //[Tooltip("The amount of forward grip for this tyre")]
    //[SerializeField] private float forwardGrip;
    //[Tooltip("The amount of sideways grip for this tyre")]
    //[SerializeField] private float sidewaysGrip;
    [Tooltip("The factor of the tyres warming up")]
    [SerializeField] private float heatFactor;
    [Tooltip("The multiplier that decides how much effect the speed has to the cooling of the wheels")]
    [SerializeField] private float airCoolingFactor;
    [Tooltip("The factor of cooling down the tyre temperature. (if the car is stationary this is the only thing affecting tyre temp)")]
    [SerializeField] private float coolDownFactor;

    public float tijdelijk;

    private float extraTemp;
    private float endTemp;

    private float timer;

    #endregion

    #region start and update

    public void Start()
    {
        ChangeCompound();
    }

    public void FixedUpdate()
    {
        TempCalculation();
        PressureCalculation();
        //GripCalculation();
    }

    #endregion

    #region change tyre compound

    public void ChangeCompound()
    {

    }

    #endregion

    #region temperature calculation

    public void TempCalculation()
    {
        float tempToAdd = 0;
        WheelHit hit;

        if (wheelCollider.GetGroundHit(out hit))
        {
            tempToAdd += airTemperature * (hit.sidewaysSlip * Mathf.Abs(hit.forwardSlip));
        }

        float degreesCooling = rb.velocity.magnitude * airCoolingFactor + coolDownFactor;
        extraTemp += tempToAdd * heatFactor + -degreesCooling; //still need to add the pressureToTempFactor here (should be * at the end)
        endTemp = airTemperature + extraTemp;

        if(endTemp < airTemperature)
        {
            endTemp = airTemperature;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = tijdelijk;
            print(endTemp);
            print(degreesCooling);
            print(tempToAdd);
        }
    }

    #endregion

    #region pressure calculation

    public void PressureCalculation()
    {
        //tire temp > graph > tire pressure

        //if pressure is lower the heat creation is higher
        //tire pressure > graph > pressureToTempFactor
    }

    #endregion

    #region grip calculation

    public void GripCalculation()
    {
        float totalGrip = 0;

        WheelHit hit;
        float surfaceGrip = 0;

        if (wheelCollider.GetGroundHit(out hit))
        {
            surfaceGrip = hit.collider.GetComponent<SurfaceGrip>().gripFactor;
        }

        //tire pressure > graph > pressureGripFactor
        //tire temps > graph > tempsGripFactor
        //surfaceGrip * tempsGripFactor * pressureGripFactor * grip

        totalGrip = surfaceGrip;
    }

    #endregion
}
