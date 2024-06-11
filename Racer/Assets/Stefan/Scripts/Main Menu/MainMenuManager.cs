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
    public ScreenEvents mapSelectionEvents;
    public ScreenEvents optionsEvents;


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

        bool inMainWindow = State != MenuState.StartScreen && State != MenuState.InputScreen && State != MenuState.OptionScreen;

        mainWindow.Toggle (inMainWindow);
        startWindow.Toggle (State == MenuState.StartScreen || State == MenuState.InputScreen );
        optionsWindow.Toggle (State == MenuState.OptionScreen);
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
