using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [SerializeField] private Transform[] placesToCycle;
    private int currentCam;

    public void Start()
    {
        ChangeCamera();
    }

    public void Update()
    {
        //temporary control code. should be changed to the new input system soon
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeCamera();
        }
    }

    #region change cam

    public void ChangeCamera()
    {
        currentCam += 1;

        if (currentCam >= placesToCycle.Length)
        {
            currentCam = 0;
        }

        transform.position = placesToCycle[currentCam].position;
        transform.rotation = placesToCycle[currentCam].rotation;
    }

    #endregion
}
