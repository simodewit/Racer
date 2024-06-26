using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainScreenInput : MonoBehaviour
{
    [Header("References")]
    public PlayerInput input;
    public CarPreview carPreview;

    public void Options (InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.ToOptionsMenu ();
    }

    public void RotateCar (InputAction.CallbackContext context )
    {
        carPreview.RotationInput (context);
    }

    public void SelectCar (InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.CarSelection);
    }

    public void SelectMap (InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.MapSelection);
    }

    public void StartGame (InputAction.CallbackContext context )
    {
        Debug.Log ("Start Game");
    }

    public void OnScreenEnable ( )
    {
        Debug.Log ("Enable");
        carPreview.ResetView ( );
        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        Debug.Log ("Whatever");
        input.enabled = false;
    }
}
