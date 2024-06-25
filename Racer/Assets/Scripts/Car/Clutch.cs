using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Clutch : MonoBehaviour
{
    [SerializeField] private UnrealisticEngine engine;
    [SerializeField] private GearBox gearBox;

    [HideInInspector] public float outputTorque;
    [HideInInspector] public float clutchAxis;

    public void FixedUpdate()
    {
        if (gearBox.currentGear == 0)
        {
            outputTorque = 0;
        }
        else
        {
            outputTorque = engine.outputTorque * (1 - clutchAxis);
        }
    }

    public void ClutchInput(InputAction.CallbackContext context)
    {
        clutchAxis = context.ReadValue<float>();
    }
}
