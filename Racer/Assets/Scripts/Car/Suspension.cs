using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The tyre script")]
    [SerializeField] private Tyre tyre;

    [Header("Spring")]
    [Tooltip("The stiffness of the spring")]
    [SerializeField] private float spring;
    [Tooltip("The stiffness of the damper")]
    [SerializeField] private float damper;
    [Tooltip("The amount of movement the spring has")]
    public float springTravel; // has to be public for the tyre script
    [Tooltip("The ride height offset from the standard ride height place. this lowers or raises the tyre")]
    [SerializeField] private float rideHeight;
    [Tooltip("The progression of the stiffness of the spring calculated from its position in its travel"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;

    //get variables
    [HideInInspector] public Transform springTargetPos;
    [HideInInspector] public float distanceInSpring;
    [HideInInspector] public float springForce;

    //private variables
    private Rigidbody tyreRb;
    private Vector3 startPos;

    #endregion

    #region start and update

    public void Start()
    {
        tyreRb = tyre.GetComponent<Rigidbody>();
        MakeTargetPos();
    }

    public void FixedUpdate()
    {
        Spring();
    }

    public void Update()
    {
        RideHeight();
    }

    #endregion

    #region startPosition

    private void MakeTargetPos()
    {
        //create the target position
        startPos = tyre.transform.localPosition;
        springTargetPos = new GameObject().transform;
        springTargetPos.parent = transform.parent;
        springTargetPos.localPosition = startPos;
        springTargetPos.name = "SpringTargetPos";
    }

    #endregion

    #region ride height

    private void RideHeight()
    {
        springTargetPos.localPosition = startPos + new Vector3(0, rideHeight, 0);
    }

    #endregion

    #region suspension

    private void Spring()
    {
        //calculate the distance of the wheel position to the target position
        if (tyre.isGrounded)
        {
            distanceInSpring = springTargetPos.localPosition.y - tyre.transform.localPosition.y;
        }
        else
        {
            distanceInSpring = (springTargetPos.localPosition.y - springTravel / 2) - tyre.transform.localPosition.y;
        }

        //calculate the value the force needs according to the spring progression graph
        float graphValue = Mathf.Abs(distanceInSpring) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        //calculate the force that should be applied to the car
        Vector3 wheelPlace = tyre.transform.TransformPoint(tyre.transform.localPosition);
        float force = (distanceInSpring * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        //calculate the place where the force towards the car should be added
        Vector3 offset = new Vector3(0, springTravel / 2, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(springTargetPos.localPosition + offset);

        //calculate force to hold the car upwards
        Vector3 carForce = -carRb.transform.up * force;
        springForce = force;

        //calculate force to keep the wheel to the target position
        float weightFactor = tyreRb.mass / carRb.mass;
        Vector3 wheelForce = tyre.transform.up * force * weightFactor;

        //
        // if you place a print here for the carForce it can show more than only the y axis, could be a bug.
        //

        //apply the forces
        if (tyre.isGrounded)
        {
            carRb.AddForceAtPosition(carForce, forcePoint);
        }

        tyreRb.AddForce(wheelForce);
    }

    #endregion
}
