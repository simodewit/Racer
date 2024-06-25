using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PlayerInput input;

    [SerializeField]
    private PauseButton[] buttons;

    [Header ("Animation Settings")]
    public float animationTime = 0.2f;
    public float minWidth, maxWidth;
    public Color unselectedBGColor, selectedBGColor;
    public Color unselectedLinesColor, selectedLinesColor;
    public Color unselectedTextColor, selectedTextColor;

    private int _selectedIndex;
    private float _buttonTimer;
    [SerializeField]
    private bool _paused;
    private void Update ( )
    {
        HandleButtonsAnimations ( );
    }

    private void HandleButtonsAnimations ( )
    {
        if ( _paused )
        {
            _buttonTimer += Time.unscaledDeltaTime;

            float progress = Mathf.InverseLerp (0, animationTime, _buttonTimer);
            UpdateButtons (progress);
        }

    }
    public void OnGamePaused ( )
    {
        SetButtonIndex (0);
        UpdateButtons (1);
        _paused = true;
    }

    public void OnGameContinue ( )
    {
        _paused = false;
    }

    void SetButtonIndex ( int index )
    {
        int newValue = Mathf.Clamp (index, 0, buttons.Length - 1);
        if ( _selectedIndex == newValue )
            return;

        _selectedIndex = newValue;

        _buttonTimer = 0;
    }

    public void ButtonNavigation ( InputAction.CallbackContext context )
    {
        if ( context.phase == InputActionPhase.Started )
        {

            float input = context.ReadValue<float> ( );

            if ( input > 0 )
            {
                SetButtonIndex (_selectedIndex - 1);
            }
            else if ( input < 0 )
            {
                SetButtonIndex (_selectedIndex + 1);
            }
        }
    }

    public void SelectButton ( InputAction.CallbackContext context )
    {
        buttons[_selectedIndex].onButtonClicked.Invoke ( );
    }

    void UpdateButtons ( float progress )
    {
        for ( int i = 0; i < buttons.Length; i++ )
        {
            var button = buttons[i];
            bool selected = _selectedIndex == i;

            //Update width
            var size = button.transform.sizeDelta;
            button.transform.sizeDelta = Vector2.Lerp (size, new (selected ? maxWidth : minWidth, size.y), progress);

            //Update BG
            button.backgroundImage.color = Color.Lerp (button.backgroundImage.color, selected ? selectedBGColor : unselectedBGColor, progress);

            //Update Lines
            button.linesImage.color = Color.Lerp (button.linesImage.color, selected ? selectedLinesColor : unselectedLinesColor, progress);

            //Update Text
            button.label.color = Color.Lerp (button.label.color, selected ? selectedTextColor : unselectedTextColor, progress);
        }
    }

    public void OnScreenEnable ( )
    {
        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        input.enabled = false;
    }


    [System.Serializable]
    class PauseButton
    {
        public RectTransform transform;
        public Image backgroundImage;
        public Image linesImage;
        public TextMeshProUGUI label;

        public UnityEvent onButtonClicked;
    }
}
