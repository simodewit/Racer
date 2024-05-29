using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelController : MonoBehaviour
{
    #region variables

    [Header("Main info")]
    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The ride height offset from the standard ride height place. this lowers or raises the tyre")]
    [SerializeField] private float rideHeight;
    // ^^^^^^ can only be set at start of the game at this moment ^^^^^^

    [Header("Spring")]
    [Tooltip("The stiffness of the spring")]
    [SerializeField] private float spring;
    [Tooltip("The stiffness of the damper")]
    [SerializeField] private float damper;
    [Tooltip("The amount of movement the spring has")]
    [SerializeField] private float springTravel;
    [Tooltip("The progression of the stiffness of the spring calculated from its position in its travel"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;

    [Header("General Grip")]
    [Tooltip("The offset you can give to the wheel. (X = null, Y = toe, Z = camber)")]
    [SerializeField] private Vector3 wheelOffset;
    [Tooltip("The in general grip factor. keep this 1 for standard settings")]
    [SerializeField] private float gripFactor;

    [Header("sideways grip")]
    [Tooltip("The grip curve of the tyre that decides how much grip you have at certain slip angles"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve gripCurve;

    //get variables
    [HideInInspector] public Transform springTargetPos;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float rpm;
    [HideInInspector] public float radius;

    //set variables
    [HideInInspector] public float motorTorque;
    [HideInInspector] public float brakeTorque;
    [HideInInspector] public float steerAngle;

    //private variables
    private Rigidbody rb;
    private List<GameObject> colliders = new List<GameObject>();
    private float distanceInSpring;
    private float springForce;

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
        ForwardGrip();
        StoppingGrip();
        RPM();
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
        Vector3 placeToCreate = transform.localPosition + new Vector3(0, rideHeight, 0);
        springTargetPos = new GameObject().transform;
        springTargetPos.parent = transform.parent;
        springTargetPos.localPosition = placeToCreate;
        springTargetPos.name = "SpringTargetPos";
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
        ClampPosition();
        ClampRotation();
        ClampVelocity();
    }

    private void ClampPosition()
    {
        Vector3 clamp = transform.localPosition;
        float startPos = springTargetPos.localPosition.y;

        clamp.y = Mathf.Clamp(clamp.y, (-springTravel / 2) + startPos, (springTravel / 2) + startPos);
        clamp.x = springTargetPos.localPosition.x;
        clamp.z = springTargetPos.localPosition.z;

        transform.localPosition = clamp;
    }

    private void ClampRotation()
    {
        Vector3 localEulers = transform.localEulerAngles;

        if (transform.localPosition.x < 0)
        {
            localEulers.y = steerAngle + wheelOffset.y;
            localEulers.z = wheelOffset.z;
        }
        else
        {
            localEulers.y = steerAngle - wheelOffset.y;
            localEulers.z = -wheelOffset.z;
        }

        transform.localEulerAngles = localEulers;
    }

    private void ClampVelocity()
    {
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
        if (isGrounded)
        {
            distanceInSpring = springTargetPos.localPosition.y - transform.localPosition.y;
        }
        else
        {
            distanceInSpring = (springTargetPos.localPosition.y - springTravel / 2) - transform.localPosition.y;
        }

        //calculate the value the force needs according to the spring progression graph
        float graphValue = Mathf.Abs(distanceInSpring) / (springTravel / 2);
        float springProgression = springCurve.Evaluate(graphValue);

        //calculate the force that should be applied to the car
        Vector3 wheelPlace = transform.TransformPoint(transform.localPosition);
        float force = (distanceInSpring * spring * springProgression) - (carRb.GetPointVelocity(wheelPlace).y * -damper);

        //calculate the place where the force towards the car should be added
        Vector3 offset = new Vector3(0, springTravel / 2, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(springTargetPos.localPosition + offset);

        //calculate force to hold the car upwards
        Vector3 carForce = -carRb.transform.up * force;
        springForce = force;

        //calculate force to keep the wheel to the target position
        float weightFactor = rb.mass / carRb.mass;
        Vector3 wheelForce = transform.up * force * weightFactor;

        //apply the forces
        if (isGrounded)
        {
            carRb.AddForceAtPosition(carForce, forcePoint);
        }

        rb.AddForce(wheelForce);
    }

    #endregion

    #region sideways grip

    private void SideWaysGrip()
    {
        if (!isGrounded)
        {
            return;
        }

        //calculate the place of the contact patch of the tyre
        Vector3 offset = new Vector3(0, distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(springTargetPos.localPosition + offset);

        //get the direction the car wants to go at the tyre position
        Vector3 tyreVelocity = carRb.GetPointVelocity(forcePoint);

        //calculate how much of the velocity is in the sideways axis of the tyre
        float dotProduct = Vector3.Dot(transform.right, tyreVelocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);
        
        //calculate the amount of force that should be applied
        float forceToPush = carRb.mass * (tyreVelocity.magnitude * dotProduct);

        //calculate available grip of tyre
        float curveProduct = gripCurve.Evaluate(Mathf.Abs(dotProduct));

        //calculate the amount of force from the rest of the car
        float weightFactor = springForce / ((carRb.mass * Physics.gravity.y) / 4);

        //add al the modifiers for the grip
        forceToPush = forceToPush * gripFactor * curveProduct * weightFactor;

        //put the force to the right direction
        Vector3 forceDirection = -transform.right * forceToPush;

        //apply the force to the car
        carRb.AddForceAtPosition(forceDirection, forcePoint);
    }

    #endregion

    #region forward grip

    private void ForwardGrip()
    {
        if (!isGrounded)
        {
            return;
        }

        //calculate the place of the contact patch of the tyre
        Vector3 offset = new Vector3(0, distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(springTargetPos.localPosition + offset);

        Vector3 torque = carRb.transform.forward * (motorTorque / radius);

        //add modifiers

        if (torque != Vector3.zero)
        {
            carRb.AddForceAtPosition(torque, forcePoint);
        }
    }

    private void StoppingGrip()
    {
        if (!isGrounded)
        {
            return;
        }

        //calculate the place of the contact patch of the tyre
        Vector3 offset = new Vector3(0, distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(springTargetPos.localPosition + offset);

        //calculate how much of the velocity is in the forward axis of the tyre
        float dotProduct = Vector3.Dot(transform.forward, carRb.velocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);
        float velocity = carRb.velocity.magnitude * dotProduct;

        Vector3 torque = Vector3.zero;

        if (velocity > 0)
        {
            torque = -carRb.transform.forward * (brakeTorque / radius);
        }
        else
        {
            torque = carRb.transform.forward * (brakeTorque / radius);
        }

        //add modifiers

        if (torque != Vector3.zero)
        {
            carRb.AddForceAtPosition(torque, forcePoint);
        }
    }

    #endregion

    #region calculate rpm

    private void RPM()
    {
        //calculate a dot value for the velocity
        float dotProduct = Vector3.Dot(transform.forward, carRb.velocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);

        //calculate the velocity
        float velocity = carRb.velocity.magnitude * dotProduct;

        //calculate the circumfrence
        float circumfrence = 2 * Mathf.PI * radius;

        //calculate rpm
        float currentRPM = velocity / circumfrence * 60;

        //add modifiers

        //add the rpm's
        rpm = currentRPM;
    }

    #endregion
}
