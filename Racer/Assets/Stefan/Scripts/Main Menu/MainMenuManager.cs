using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuManager : MonoBehaviour
{
    public enum MenuState
    {
        StartScreen,
        InputScreen,
        MainMenu,
        CarSelection,
        MapSelection
    }

    [SerializeField]
    private MenuState m_state;

    [Header ("Screens")]
    public UIGroup startScreen;
    public UIGroup mainScreen;
    public UIGroup carSelectionScreen;
    public UIGroup mapSelectionScreen;
    public UIGroup userInputScreen;

    public UIGroup mainWindow, startWindow;
    public MenuState State
    {
        get
        {
            return m_state;
        }
        private set
        {
            if ( value == m_state )
                return;

            m_state = value;

            OnMenuStateChanged ( );
        }
    }

    public UnityEvent<MenuState> onMenuStateChanged;

    private void OnMenuStateChanged ( )
    {
        onMenuStateChanged.Invoke (m_state);

        UIGroup activeScreen = m_state switch
        {
            MenuState.StartScreen => startScreen,
            MenuState.InputScreen => userInputScreen,
            MenuState.MainMenu => mainScreen,
            MenuState.CarSelection => carSelectionScreen,
            MenuState.MapSelection => mapSelectionScreen,
            _ => null
        };

        if ( activeScreen != null )
            ToggleScreen (activeScreen);

        bool inMainWindow = State != MenuState.StartScreen && State != MenuState.InputScreen;

        mainWindow.Toggle (inMainWindow);
        startWindow.Toggle (!inMainWindow);
    }

    public void ToggleScreen(UIGroup activeScreen )
    {
        var screens = new UIGroup[] { startScreen, mainScreen, carSelectionScreen, mapSelectionScreen, userInputScreen };

        foreach ( var screen in screens )
        {
            if(screen == activeScreen )
            {
                screen.Toggle (true);
                continue;
            }
            screen.Toggle (false);
        }
    }

    public void StartScreenContinue ( )
    {
        if ( Authenticator.IsSignedIn ) // Continue to main screen
        {
            mainScreen.Toggle (true);
        }
        else // Let user select a name
        {
            userInputScreen.Toggle (true);
        }

        startScreen.Toggle (false);

        State = MenuState.MainMenu;
    }

    [ProButton]
    public void SetMenuState(MenuState state )
    {
        State = state;
    }
}
