using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAero : MonoBehaviour
{
    #region variables

    [Tooltip("The amount of m2 of the front diffuser")]
    [SerializeField] private float frontDiffuser;
    [Tooltip("The amount of m2 of the rear wing")]
    [SerializeField] private float rearWing;
    [Tooltip("The place where the front downforce is applied")]
    [SerializeField] private Transform frontPlace;
    [Tooltip("The place where the rear downforce is applied")]
    [SerializeField] private Transform rearPlace;

    [Header("Read data")]
    public float totalDownForce;

    private Rigidbody rb;

    #endregion

    #region start and update

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        ApplyDownForce();
    }

    #endregion

    #region apply downforce

    public void ApplyDownForce()
    {
        float speed = rb.velocity.sqrMagnitude;

        Vector3 downForceFront = -transform.up * (frontDiffuser / 100 * speed);
        Vector3 downForceRear = -transform.up * (rearWing / 100 * speed);

        rb.AddForceAtPosition(downForceFront, frontPlace.position);
        rb.AddForceAtPosition(downForceRear, rearPlace.position);

        totalDownForce = downForceFront.y + downForceRear.y;
    }

    #endregion
}
