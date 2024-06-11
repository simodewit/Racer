using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapSelector : MonoBehaviour
{
    [Header ("Input")]
    public PlayerInput input;

    public void Back (InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.MainMenu);
    }

    public void Options(InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.ToOptionsMenu ( );
    }

    public void OnScreenEnable ( )
    {
        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        input.enabled = false;
    }
}
