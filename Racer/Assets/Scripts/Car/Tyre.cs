using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Tyre : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [Tooltip("The suspension script")]
    [SerializeField] private Suspension suspension;
    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;

    [Header("General Grip")]
    [Tooltip("The offset you can give to the wheel. (X = null, Y = toe, Z = camber)")]
    [SerializeField] private Vector3 wheelOffset;
    [Tooltip("The in general grip factor. keep this 1 for standard settings")]
    [SerializeField] private float gripFactor;

    [Header("sideways grip")]
    [Tooltip("The grip curve of the tyre that decides how much grip you have at certain slip angles"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve gripCurve;

    //get variables
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float rpm;
    [HideInInspector] public float radius;
    [HideInInspector] public float totalSidewayGrip;

    //set variables
    [HideInInspector] public float motorTorque;
    [HideInInspector] public float brakeTorque;
    [HideInInspector] public float steerAngle;

    //private variables
    private List<SurfaceType> surfaces = new List<SurfaceType>();
    private Rigidbody rb;
    private float surfaceGrip;
    private float surfaceResistance;
    private float minimumResistance;

    #endregion

    #region start and update

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        SideWaysGrip();
        ForwardGrip();
        StoppingGrip();
        SurfaceResistance();
    }

    public void Update()
    {
        RPM();
        Clamps();
        CollisionCheck();
    }

    #endregion

    #region clamps

    private void Clamps()
    {
        ClampPosition();
        ClampRotation();
        ClampVelocity();
    }

    private void ClampPosition()
    {
        Vector3 clamp = transform.localPosition;
        float startPos = suspension.springTargetPos.localPosition.y;

        clamp.y = Mathf.Clamp(clamp.y, (-suspension.springTravel / 2) + startPos, (suspension.springTravel / 2) + startPos);
        clamp.x = suspension.springTargetPos.localPosition.x;
        clamp.z = suspension.springTargetPos.localPosition.z;

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

        transform.Rotate(new Vector3(100, 0, 0) * Time.deltaTime);
    }

    private void ClampVelocity()
    {
        Vector3 vel = rb.velocity;
        vel.x = 0;
        vel.z = 0;
        rb.velocity = vel;
    }

    #endregion

    #region calculate rpm

    private void RPM()
    {
        if (radius == float.NaN)
        {
            return;
        }

        //calculate a dot value for the velocity
        float dotProduct = Vector3.Dot(transform.forward, carRb.velocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);

        //calculate the velocity
        float velocity = carRb.velocity.magnitude * dotProduct;

        //calculate the circumfrence
        float circumfrence = 2 * Mathf.PI * radius;

        //calculate rpm
        float currentRPM = velocity / circumfrence * 60;

        //add the rpm's
        rpm = currentRPM;
    }

    #endregion

    #region collision

    public void OnCollisionEnter(Collision collision)
    {
        if (radius == 0)
        {
            radius = Vector3.Distance(transform.position, collision.GetContact(0).point);
        }

        if (collision.transform.GetComponent<SurfaceType>() != null)
        {
            surfaces.Add(collision.transform.GetComponent<SurfaceType>());
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.transform.GetComponent<SurfaceType>() != null)
        {
            surfaces.Remove(collision.transform.GetComponent<SurfaceType>());
        }
    }

    private void CollisionCheck()
    {
        if (surfaces.Count == 0)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;

            float totalGrip = 0;
            float totalResistance = 0;
            float minResistance = 0;
            foreach (SurfaceType surface in surfaces)
            {
                totalGrip += surface.grip / 100;
                totalResistance += surface.resistance;
                minResistance += surface.minimumResistance;
            }

            surfaceGrip = totalGrip / surfaces.Count;
            surfaceResistance = totalResistance / surfaces.Count;
            minimumResistance = minResistance / surfaces.Count;
        }
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
        Vector3 offset = new Vector3(0, suspension.distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(suspension.springTargetPos.localPosition + offset);

        //get the direction the car wants to go at the tyre position
        Vector3 tyreVelocity = carRb.GetPointVelocity(forcePoint);

        //calculate how much of the velocity is in the sideways axis of the tyre
        float dotProduct = Vector3.Dot(transform.right, tyreVelocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);

        //calculate the amount of force that should be applied
        float idealForce = carRb.mass * (tyreVelocity.magnitude * dotProduct);

        //calculate available grip of tyre
        float curveProduct = gripCurve.Evaluate(Mathf.Abs(dotProduct));

        //calculate the amount of force from the rest of the car
        float weightFactor = suspension.springForce / (carRb.mass * Physics.gravity.y / 4);

        //add al the modifiers for the grip
        float forceToPush = idealForce * gripFactor * curveProduct * weightFactor * surfaceGrip;

        //put the force to the right direction
        Vector3 forceDirection = -transform.right * forceToPush;

        //apply the force to the car
        carRb.AddForceAtPosition(forceDirection, forcePoint);

        //calculate the total grip of the vehicle
        totalSidewayGrip = forceToPush / idealForce;
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
        Vector3 offset = new Vector3(0, suspension.distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(suspension.springTargetPos.localPosition + offset);

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
        Vector3 offset = new Vector3(0, suspension.distanceInSpring - radius, 0);
        Vector3 forcePoint = carRb.transform.TransformPoint(suspension.springTargetPos.localPosition + offset);

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

        carRb.AddForceAtPosition(torque, forcePoint);
    }

    #endregion

    #region surfaceResistance

    private void SurfaceResistance()
    {
        float resistance = surfaceResistance * carRb.velocity.magnitude;

        if (resistance < minimumResistance)
        {
            resistance = minimumResistance;
        }

        Vector3 resistanceDirection = -carRb.velocity.normalized * resistance;

        carRb.AddForceAtPosition(resistanceDirection, transform.position);
    }

    #endregion
}
