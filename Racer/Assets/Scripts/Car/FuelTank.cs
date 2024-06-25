using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FuelTank : MonoBehaviour
{
    [Tooltip("The amount of fuel that the car has in liters")]
    public float fuel = 50;
    [Tooltip("The amount of Megajoules per liter that this fuel can produce when ignited. (gasoline = 34.2)")]
    public float energyContent = 34.2f;

    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        rb.mass = fuel;
    }
}
