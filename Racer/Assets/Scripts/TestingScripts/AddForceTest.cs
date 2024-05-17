using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceTest : MonoBehaviour
{
    public bool shoot;
    public Rigidbody rb;
    public Vector3 forceToAdd;
    public Vector3 offsetPlace;

    public void Update()
    {
        if (shoot)
        {
            shoot = false;
            rb.AddForceAtPosition(forceToAdd, offsetPlace + transform.position);
        }

        print(rb.velocity.magnitude);
    }
}
