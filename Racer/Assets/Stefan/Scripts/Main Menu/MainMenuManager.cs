using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ScreenEvents
{
    public UnityEvent onScreenEnabled;
    public UnityEvent onScreenDisabled;
}

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager _instance;
    public static MainMenuManager Instance
    {
        get
        {
            if(_instance == null )
            {
                _instance = FindObjectOfType<MainMenuManager> ( );
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public enum MenuState
    {
        StartScreen,
        InputScreen,
        MainMenu,
        CarSelection,
        MapSelection,
        OptionScreen,
    }

    [SerializeField]
    private MenuState m_state;

    [Header ("Screens")]
    public UIGroup startScreen;
    public UIGroup mainScreen;
    public UIGroup carSelectionScreen;
    public UIGroup mapSelectionScreen;
    public UIGroup userInputScreen;

    public UIGroup mainWindow, startWindow, optionsWindow;
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

    [Header("Events")]
    public UnityEvent<MenuState> onMenuStateChanged;

    public ScreenEvents carSelectionEvents;
    public ScreenEvents startWindowEvents;
    public ScreenEvents mainScreenEvents;
    public ScreenEvents mapSelectionEvents;
    public ScreenEvents optionsEvents;

    //Private Variables
    private MenuState _menuBeforeOptions;


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
            MenuState.OptionScreen => optionsWindow,
            _ => null
        };

        if ( activeScreen != null )
            ToggleScreen (activeScreen);

        bool inMainWindow = State != MenuState.StartScreen && State != MenuState.InputScreen && State != MenuState.OptionScreen;

        if ( inMainWindow && !mainWindow.isActive )
        {
            mainScreenEvents.onScreenEnabled?.Invoke ( );
        }
        else if(!inMainWindow && mainWindow.isActive )
        {
            mainScreenEvents.onScreenDisabled?.Invoke ( );
        }

        mainWindow.Toggle (inMainWindow);
        startWindow.Toggle (State == MenuState.StartScreen || State == MenuState.InputScreen );
        optionsWindow.Toggle (State == MenuState.OptionScreen);
    }

    /// <summary>
    /// Toggles a screen and handles its events
    /// </summary>
    /// <param name="activeScreen">The screen that should be enabled</param>
    public void ToggleScreen(UIGroup activeScreen )
    {
        var screens = new UIGroup[] { startScreen, mainScreen, carSelectionScreen, mapSelectionScreen, userInputScreen };

        foreach ( var screen in screens )
        {
            if(screen == activeScreen )
            {
                if(screen.isActive == false ) // Call enable screen events
                {
                    var events = GetScreenEvents (screen);
                    events?.onScreenEnabled.Invoke ( );
                }

                screen.Toggle (true);
                continue;
            }
            if ( screen.isActive) // Call disable screen
            {
                var events = GetScreenEvents (screen);
                events?.onScreenDisabled.Invoke ( );
            }

            screen.Toggle (false);
        }
    }

    /// <summary>
    /// Gets the screen events for an individual UIGroup
    /// </summary>
    /// <param name="group">The group to find events for</param>
    /// <returns>The events of the screen, null if none found</returns>
    public ScreenEvents GetScreenEvents (UIGroup group)
    {
        if ( group == startScreen )
            return startWindowEvents;
        if ( group == mapSelectionScreen )
            return mapSelectionEvents;
        if ( group == carSelectionScreen )
            return carSelectionEvents;
        if ( group == optionsWindow )
            return optionsEvents;
        if(group == mainScreen)
            return mainScreenEvents;

        return null;
    }

    #region Options

    public void ToOptionsMenu ( )
    {
        _menuBeforeOptions = State;

        State = MenuState.OptionScreen;

        optionsEvents.onScreenEnabled.Invoke ( );
    }

    public void ExitOptions ( )
    {
        State = _menuBeforeOptions;

        optionsWindow.Toggle (false);

        _menuBeforeOptions = MenuState.MainMenu;
        optionsEvents.onScreenDisabled.Invoke ( );
    }

    #endregion

    [ProButton]
    public void SetMenuState(MenuState state )
    {
        State = state;
    }


}
