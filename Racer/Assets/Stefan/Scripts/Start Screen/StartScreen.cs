using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreen : MonoBehaviour
{

    public PlayerInput input;
    public TMP_InputField inputField;
    public UIGroup continueGroup, nameInputScreen;

    private bool inInputScreen;

    public void ContinueGame ( )
    {
        if ( !input.enabled )
            return;

        if ( inInputScreen )
            return;

        inInputScreen = true;

        if ( Authenticator.IsSignedIn )
        {
            MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.MainMenu);
        }
        else
        {
            continueGroup.Toggle (false);
            nameInputScreen.Toggle (true);
        }
    }

    public void CreateUser ( )
    {
        if ( !input.enabled )
            return;

        if ( !inInputScreen )
            return;

        Authenticator.CreateUser (inputField.text);
        MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.MainMenu);
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
