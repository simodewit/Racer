using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarCamera : MonoBehaviour
{
    [SerializeField] private Transform[] placesToCycle;
    private int currentCam;

    public void Start()
    {
        Change();
    }

    public void ChangeCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Change();
        }
    }

    public void Change()
    {
        currentCam += 1;

        if (currentCam >= placesToCycle.Length)
        {
            currentCam = 0;
        }

        transform.position = placesToCycle[currentCam].position;
        transform.rotation = placesToCycle[currentCam].rotation;
    }
}
