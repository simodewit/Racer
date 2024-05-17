using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WheelController : MonoBehaviour
{
    #region variables

    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;

    [Header("Spring")]
    [SerializeField] private float springTravel;
    [Tooltip("The progression of the stiffness of the spring calculated from its position in its travel"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;
    [Tooltip("The offset you can give for the place where the spring wants to go")]
    [SerializeField] private float targetPosOffset;
    [Tooltip("The stiffness of the spring")]
    [SerializeField] private float spring;
    [Tooltip("The stiffness of the damper")]
    [SerializeField] private float damper;

    [Header("Grip")]
    [Tooltip("The offset you can give to the wheel. (X = null, Y = toe, Z = camber)")]
    [SerializeField] private Vector3 wheelOffset;

    //variables for the gizmos
    private Vector3 gizmosSpringForce;
    private Vector3 gizmosSpringApplyPos;
    private float gizmosSpringDistance;

    //other private variables
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
        Suspension();
    }

    public void Update()
    {
        Clamps();
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

    #region collision

    public void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    public void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    #endregion

    #region clamp

    private void Clamps()
    {
        //creates the clamp vector
        Vector3 clamp = transform.localPosition;

        //clamp the suspension in between the correct values
        float startPos = targetPos.localPosition.y;
        clamp.y = Mathf.Clamp(clamp.y, (-springTravel / 2) + startPos, (springTravel / 2) + startPos);

        //keeps the wheels on the correct x and z values
        clamp.x = targetPos.localPosition.x;
        clamp.z = targetPos.localPosition.z;

        //apply position and rotation values to how its needed
        transform.localPosition = clamp;
        transform.localEulerAngles = Vector3.zero + wheelOffset;

        //clamp the velocity
        Vector3 vel = rb.velocity;
        vel.x = 0;
        vel.z = 0;
        rb.velocity = vel;
    }

    #endregion

    #region suspension

    private void Suspension()
    {
        //calculate the distance of the wheel position to the target position
        float distance = targetPos.localPosition.y - transform.localPosition.y;

        //calculate the value the force needs according to the spring progression graph
        float graphValue = Mathf.Abs(distance) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        //calculate the force that should be applied to the car
        Vector3 wheelPlace = transform.TransformPoint(transform.localPosition);
        float force = (distance * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        //calculate force to hold the car upwards
        Vector3 carForce = -carRb.transform.up * force;

        Vector3 offset = new Vector3(0, springTravel / 2, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(targetPos.localPosition + offset);

        //calculate force to keep the wheel to the target position
        float weightFactor = rb.mass / carRb.mass;
        Vector3 wheelForce = -transform.up * force * weightFactor;

        //apply the forces
        if (distance < 0 && isGrounded)
        {
            print("wheel = " + transform.name + ". force = " + carForce + ". forceType = car. distance = " + distance);
            carRb.AddForceAtPosition(carForce, forcePoint);
            gizmosSpringForce = carForce;
        }
        else if (distance > 0)
        {
            print("wheel = " + transform.name + ". force = " + wheelForce + ". forceType = wheel. distance = " + distance);
            rb.AddForce(wheelForce);
            gizmosSpringForce = wheelForce;
        }
        else if (distance < 0 && !isGrounded)
        {
            print("wheel = " + transform.name + ". force = " + wheelForce + ". forceType = wheel. distance = " + distance);
            rb.AddForce(wheelForce);
            gizmosSpringForce = wheelForce;
        }
        else
        {
            print("no conditions are met");
        }

        //info for the gizmos
        gizmosSpringApplyPos = forcePoint;
        gizmosSpringDistance = distance;
    }

    #endregion

    #region grip

    //could use the dot product between the front of the car and the tyre. then add velocity according to the x value of the dot product.


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

        //places a cube on the middle of the spring
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(middleOfSpring, new Vector3(0.1f, 0.1f, 0.1f));
    }

    #endregion
}
