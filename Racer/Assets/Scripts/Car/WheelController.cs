using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelController : MonoBehaviour
{
    #region variables

    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;

    [Header("Spring")]
    [Tooltip("The stiffness of the spring")]
    [SerializeField] private float spring;
    [Tooltip("The stiffness of the damper")]
    [SerializeField] private float damper;
    [Tooltip("The amount of movement the spring has")]
    [SerializeField] private float springTravel;
    [Tooltip("The progression of the stiffness of the spring calculated from its position in its travel"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;

    [Header("Grip")]
    [Tooltip("The offset you can give to the wheel. (X = null, Y = toe, Z = camber)")]
    [SerializeField] private Vector3 wheelOffset;
    [Tooltip("The in general grip factor. keep this 1 for standard settings")]
    [SerializeField] private float gripFactor;

    [HideInInspector]
    public Transform targetPos;

    public bool debugInfo;

    //variables for the gizmos
    private Vector3 gizmosSpringForce;
    private Vector3 gizmosSpringApplyPos;
    private Vector3 gizmosGripPoint;
    private Vector3 gizmosGripDirection;

    //other private variables
    private Rigidbody rb;
    private bool isGrounded;
    private List<GameObject> colliders = new List<GameObject>();
    private float springDistance;
    private float radius;

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
        SideWaysGrip();
    }

    public void Update()
    {
        CollisionCheck();
        Clamps();
    }

    #endregion

    #region startPosition

    private void MakeTargetPos()
    {
        //create the target position
        Vector3 placeToCreate = transform.localPosition;
        targetPos = new GameObject().transform;
        targetPos.parent = transform.parent;
        targetPos.localPosition = placeToCreate;
        targetPos.name = "SpringTargetPos";
    }

    #endregion

    #region collision

    public void OnCollisionEnter(Collision collision)
    {
        if (radius == 0)
        {
            radius = Vector3.Distance(transform.position, collision.GetContact(0).point);
        }

        colliders.Add(collision.gameObject);
    }

    public void OnCollisionExit(Collision collision)
    {
        colliders.Remove(collision.gameObject);
    }

    private void CollisionCheck()
    {
        if (colliders.Count == 0)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
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
        springDistance = 0;

        //calculate the distance of the wheel position to the target position
        if (isGrounded)
        {
            springDistance = targetPos.localPosition.y - transform.localPosition.y;
        }
        else
        {
            springDistance = (targetPos.localPosition.y - springTravel / 2) - transform.localPosition.y;
        }

        //calculate the value the force needs according to the spring progression graph
        float graphValue = Mathf.Abs(springDistance) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        //calculate the force that should be applied to the car
        Vector3 wheelPlace = transform.TransformPoint(transform.localPosition);
        float force = (springDistance * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        //calculate the place where the force towards the car should be added
        Vector3 offset = new Vector3(0, springTravel / 2, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(targetPos.localPosition + offset);

        //calculate force to hold the car upwards
        Vector3 carForce = -carRb.transform.up * force;

        //calculate force to keep the wheel to the target position
        float weightFactor = rb.mass / carRb.mass;
        Vector3 wheelForce = transform.up * force * weightFactor;

        //apply the forces
        if (isGrounded)
        {
            carRb.AddForceAtPosition(carForce, forcePoint);
            gizmosSpringForce = carForce;
        }
        else
        {
            rb.AddForce(wheelForce);
            gizmosSpringForce = wheelForce * 1000;
        }

        //info for the gizmos
        gizmosSpringApplyPos = forcePoint;
    }

    #endregion

    #region sideways grip

    private void SideWaysGrip()
    {
        if (!isGrounded)
        {
            gizmosGripDirection = new Vector3(0, 0, 0);
            return;
        }

        //calculate the place of the contact patch of the tyre
        Vector3 offset = new Vector3(0, springDistance - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(targetPos.localPosition + offset);

        //get the direction the tyre wants to go
        Vector3 carRightSide = carRb.transform.TransformPoint(transform.right);

        //get the cars velocity on the position of the tyre
        Vector3 velocity = carRb.GetPointVelocity(forcePoint);

        //calculate the amount of force of the velocity is against the tyre
        float angleToCorrect = Vector3.Dot(carRightSide, velocity);
        angleToCorrect = Mathf.Clamp(angleToCorrect, -1, 1);

        //calculate the amount of force that should be applied
        float forceToPush = carRb.mass * (carRb.velocity.magnitude * angleToCorrect);

        //add al the modifiers for the grip
        forceToPush = forceToPush * gripFactor;

        //put the force to the right direction
        Vector3 forceDirection = transform.right * forceToPush;

        //apply the force to the car
        carRb.AddForceAtPosition(forceDirection, forcePoint);

        //gizmos info
        gizmosGripDirection = forceDirection;
        gizmosGripPoint = forcePoint;
    }

    #endregion

    #region forward grip

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
        Vector3 wheelPos = middleOfSpring + new Vector3(0, springDistance, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(wheelPos, new Vector3(0.1f, 0.1f, 0.1f));

        //draws the forces of the springs
        Vector3 toShow = gizmosSpringForce / spring * 10 + transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, toShow);

        //places a cube on the middle of the spring
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(middleOfSpring, new Vector3(0.1f, 0.1f, 0.1f));

        //draws the forces of the springs
        Gizmos.color = Color.red;
        Gizmos.DrawLine(gizmosGripPoint, gizmosGripPoint + gizmosGripDirection * 0.1f);
    }

    #endregion
}
