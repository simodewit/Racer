using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotations : MonoBehaviour
{
    [SerializeField] private GameObject wheelModel;
    private WheelCollider wheelCollider;

    public void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    public void Update()
    {
        wheelModel.transform.localEulerAngles = new Vector3(wheelCollider.rpm / 60 * 360 * Time.deltaTime, wheelCollider.steerAngle, 0);
    }
}
