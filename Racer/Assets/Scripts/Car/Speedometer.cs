using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [Header("Should be on the object with the rigidbody.")]
    [Tooltip("The speed at wich you are going. READ ONLY")]
    public float speed;
    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        speed = rb.velocity.magnitude * 3.6f;
    }
}
