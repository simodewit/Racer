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
    [SerializeField] private float forceOffset;
    [SerializeField] private float springTravel;
    [Tooltip("The progression of the stiffness of the spring"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;
    [SerializeField] private float targetPosOffset;
    [SerializeField] private float spring;
    [SerializeField] private float damper;

    public bool debug;

    private Vector3 gizmosSpringForce;
    private Vector3 gizmosSpringApplyPos;

    private Rigidbody rb;
    private Vector3 targetPos;

    #endregion

    #region start and update

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPos = transform.localPosition + new Vector3(0, targetPosOffset, 0);
    }

    public void FixedUpdate()
    {
        Clamps();
        Suspension();
    }

    #endregion

    #region clamp

    private void Clamps()
    {
        Vector3 clamp = transform.localPosition;
        Mathf.Clamp(clamp.y, -springTravel / 2, springTravel / 2);
        clamp.x = targetPos.x;
        clamp.z = targetPos.z;
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
        float distance = targetPos.y - transform.localPosition.y;

        float graphValue = Mathf.Abs(distance) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        Vector3 wheelPlace = transform.TransformPoint(transform.localPosition);
        float force = (distance * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        Vector3 direction = -carRb.transform.up * force;

        float yAxisOffset = targetPos.y + springTravel / 2 + forceOffset;
        Vector3 forcePoint = new Vector3(targetPos.x, yAxisOffset, targetPos.z) + carRb.transform.position;
        
        carRb.AddForceAtPosition(direction, forcePoint);

        gizmosSpringForce = direction;
        gizmosSpringApplyPos = forcePoint;
    }

    #endregion

    #region gizmos

    private void OnDrawGizmos()
    {
        Vector3 toShow = gizmosSpringForce / spring * 10 + transform.position;
        Gizmos.DrawLine(transform.position, toShow);

        Gizmos.DrawCube(gizmosSpringApplyPos, new Vector3(0.1f, 0.1f, 0.1f));
    }

    #endregion
}
