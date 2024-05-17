using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    #region variables

    [SerializeField] private Rigidbody carRb;

    [Header("Spring")]
    [SerializeField] private float springTravel;
    [Tooltip("The progression of the stiffness of the spring calculated from its position in its travel"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;
    [Tooltip("The offset you can give for the place where the spring wants to go")]
    [SerializeField] private float targetPosOffset;
    [SerializeField] private float spring;
    [SerializeField] private float damper;

    public bool info;

    //variables for the gizmos
    private Vector3 gizmosSpringForce;
    private Vector3 gizmosSpringApplyPos;
    private float gizmosSpringDistance;

    private Rigidbody rb;
    private Transform targetPos;

    private bool isGrounded;

    #endregion

    #region start and update

    public void Start()
    {
        MakeTargetPos();

        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        Clamps();
        Suspension();
    }

    #endregion

    #region startPosition

    private void MakeTargetPos()
    {
        Vector3 placeToCreate = transform.localPosition + new Vector3(0, targetPosOffset, 0);
        targetPos = new GameObject().transform;
        targetPos.parent = transform.parent;
        targetPos.localPosition = placeToCreate;
        targetPos.name = "SpringTargetPos";
    }

    #endregion

    #region clamp

    private void Clamps()
    {
        Vector3 clamp = transform.localPosition;

        float startPos = targetPos.localPosition.y;
        clamp.y = Mathf.Clamp(clamp.y, (-springTravel / 2) + startPos, (springTravel / 2) + startPos);

        clamp.x = targetPos.localPosition.x;
        clamp.z = targetPos.localPosition.z;

        transform.localPosition = clamp;
        transform.localRotation = Quaternion.identity;

        Vector3 vel = rb.velocity;
        vel.x = 0;
        vel.z = 0;
        rb.velocity = vel;
    }

    #endregion

    #region suspension

    private void Suspension()
    {
        float distance = targetPos.localPosition.y - transform.localPosition.y;

        float graphValue = Mathf.Abs(distance) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        Vector3 wheelPlace = transform.TransformPoint(transform.localPosition);
        float force = (distance * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        Vector3 direction = -carRb.transform.up * force;

        Vector3 offset = new Vector3(0, springTravel / 2, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(targetPos.localPosition + offset);
        
        carRb.AddForceAtPosition(direction, forcePoint);

        //info for the gizmos
        gizmosSpringForce = direction;
        gizmosSpringApplyPos = forcePoint;
        gizmosSpringDistance = distance;

        if (info)
        {
            print("Distance = " + distance + "Force = " + force);
        }
    }

    #endregion

    #region gizmos

    //i am using gizmos for properly understanding and vizualizing the results of the system
    private void OnDrawGizmos()
    {
        //draws the place where the force is applied
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(gizmosSpringApplyPos, new Vector3(0.1f, 0.1f, 0.1f));

        //draws the lowest place of the spring
        Gizmos.color = Color.cyan;
        Vector3 lowestSpringPoint = new Vector3(gizmosSpringApplyPos.x, gizmosSpringApplyPos.y - springTravel, gizmosSpringApplyPos.z);
        Gizmos.DrawCube(lowestSpringPoint, new Vector3(0.1f, 0.1f, 0.1f));

        //draws the travel of the spring
        Gizmos.color = Color.white;
        Gizmos.DrawLine(gizmosSpringApplyPos, lowestSpringPoint);

        //draws the place where the wheel is in the travel of the spring
        Vector3 middleOfSpring = lowestSpringPoint + new Vector3(0, springTravel / 2, 0);
        Vector3 wheelPos = middleOfSpring + new Vector3(0, gizmosSpringDistance, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(wheelPos, new Vector3(0.1f, 0.1f, 0.1f));

        //draws the forces of the springs
        Vector3 toShow = gizmosSpringForce / spring * 10 + transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, toShow);
    }

    #endregion
}
