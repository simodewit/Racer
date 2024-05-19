using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceTest : MonoBehaviour
{
    public bool shoot;
    public Rigidbody rb;
    public Vector3 forceToAdd;

    public void Update()
    {
        if (shoot)
        {
            shoot = false;
            rb.AddRelativeForce(forceToAdd);
        }
    }
}
