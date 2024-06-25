using com.cyborgAssets.inspectorButtonPro;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager m_instance;
    public static OptionsManager Instance
    {
        get
        {
            if(m_instance == null )
            {
                m_instance = FindObjectOfType<OptionsManager>(false);
            }

            return m_instance;
        }
        set
        {
            m_instance = value;
        }
    }

    [SerializeField]
    List<UIOption> allOptions;

    [Header ("Settings")]
    public Sprite emptyOptionSprite;

    public float heightPerOption;

    public float minHeight, maxHeight, heightOffset, heightSmoothSpeed;

    [Header("Input Settings")]
    public float scrollDeadzone = 0.5f;
    public float dpadInputSensitivity = 1;
    public float rightJoystickSensitivity = 0.4f;

    [Range(0,0.2f)]
    public float inputCooldown = 0.07f;

    [Header ("References")]
    public Image optionImage;
    public TextMeshProUGUI optionNameText,optionDescriptionText;
    public VerticalLayoutGroup layout;
    public GameObject applyButton;
    public PlayerInput input;

    [Header ("Options")]
    public UnityEvent<OptionsData> onOptionsLoaded;
    public UnityEvent<OptionsData> onOptionsApplied;

    [Header ("Option Elements")]
    // Screen Settings
    public UISelector resolutionSelector;
    public UISelector frameRateSelector;
    public UIToggle fullscreenToggle;

    //Graphic Settings
    public UISlider gammaSlider;
    public UIToggle cloudsToggle;

    //Audio Settings
    public UISlider mainVolumeSlider;
    public UISlider uiVolumeSlider;
    public UISlider sfxVolumeSlider;
    public UISlider musicVolumeSlider;


    //Private variables
    [SerializeField]
    private int currentOptionIndex;

    float layoutVelocity;

    bool m_isDirty;

    float _cooldownTimer;
    private void Start ( )
    {
        if ( Instance == null )
        {
            Instance = this;
        }
        else if ( Instance != this )
        {
            Destroy (gameObject);
            Debug.Log ("Removed an options Window");
            return;
        }

        OnCurrentOptionIndexChanged ( );

        InitializeOptionElements ( );
    }

    private void Update ( )
    {
        HandleScrollView ( );

        _cooldownTimer -= Time.deltaTime;
    }

    #region Input

    public void ToggleInput (InputAction.CallbackContext context )
    {
        if ( context.phase != InputActionPhase.Started )
            return;

        UIOption option = allOptions[currentOptionIndex];

        if ( option == null || option.inputReceiver == null )
            return;

        option.inputReceiver.OnReceiveXInput ( );

    }

    public void LeftJoystick(InputAction.CallbackContext context )
    {
        var input = context.ReadValue<Vector2> ( );

        //Handle Scrolling

        CheckScrollInput ( input.y);
    }

    public void RightJoystick(InputAction.CallbackContext context )
    {
        var input = context.ReadValue<Vector2> ( );


        //Send input to option
        UIOption option = allOptions[currentOptionIndex];

        if ( option == null || option.inputReceiver == null )
            return;

        option.inputReceiver.OnReceiveHorizontalInput (input.x * rightJoystickSensitivity);
        option.inputReceiver.OnReceiveVerticalInput (input.y * rightJoystickSensitivity);
    }

    public void DpadInput(InputAction.CallbackContext context )
    {
        var input = context.ReadValue<Vector2> ( );

        //Handle Scrolling

        CheckScrollInput (input.y);

        //Send input to option
        UIOption option = allOptions[currentOptionIndex];

        if ( option == null || option.inputReceiver == null )
            return;

        option.inputReceiver.OnReceiveHorizontalInput (input.x * dpadInputSensitivity);
    }

    [ProButton]
    void CheckScrollInput (float input )
    {
        if ( _cooldownTimer > 0 )
            return;

        if ( Mathf.Abs(input) < scrollDeadzone ) //Not sensitive enough to scroll
            return;

        if ( input < 0 ) // scrolling down
        {
            if ( currentOptionIndex < allOptions.Count - 1 )
            {
                SetCurrentOptionIndex (currentOptionIndex + 1);
                _cooldownTimer = inputCooldown;
            }
        }
        else //Scrolling up
        {
            if ( currentOptionIndex > 0)
            {
                SetCurrentOptionIndex (currentOptionIndex - 1);
                _cooldownTimer = inputCooldown;
            }
        }
    }

    #endregion

    #region Screen Events

    public void TryGoBack ( )
    {
        //TODO: check if user has unnaplied changed
        GoBack ( );
    }

    public void GoBack ( )
    {
        //TODO: Go bbakc to previous screen
        MainMenuManager.Instance.ExitOptions ( );

    }

    public void OnScreenEnable ( )
    {
        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        input.enabled = false;
    }

    #endregion

    #region Visuals
    void HandleScrollView ( )
    {
        float target = currentOptionIndex * heightPerOption + heightOffset;

        target = Mathf.Clamp (target, minHeight, maxHeight);

        layout.padding.top = Mathf.RoundToInt(Mathf.SmoothDamp (layout.padding.top, target, ref layoutVelocity, heightSmoothSpeed));
        layout.SetLayoutVertical ( );
    }

    private void SetCurrentOptionIndex(int newIndex )
    {
        allOptions[currentOptionIndex].SetSelected (false);
        currentOptionIndex = newIndex;
        allOptions[currentOptionIndex].SetSelected (true);

        OnCurrentOptionIndexChanged ( );
    }
    private void OnCurrentOptionIndexChanged ( )
    {
        UIOption currentOption = allOptions[currentOptionIndex];

        if ( currentOption == null )
            return;

        if(currentOption.optionSprite != null )
        {
            optionImage.sprite = currentOption.optionSprite;
        }
        else
        {
            optionImage.sprite = emptyOptionSprite;
        }

        optionNameText.text = currentOption.optionName;
        optionDescriptionText.text = currentOption.optionDescription;
    }

    #endregion

    #region Options Settings

    public void InitializeOptionElements ( )
    {
        OptionsData data = OptionsData.Saved;

        //Screen Settings
        resolutionSelector.SetIndex (data.resolutionIndex);
        frameRateSelector.SetIndex (data.frameRateIndex);
        fullscreenToggle.SetValue (data.fullscreenMode);

        //Graphic Settings
        gammaSlider.SetValue (data.gamma);
        cloudsToggle.SetValue (data.VolumetricClouds);

        //Audio Settings
        mainVolumeSlider.SetValue (data.mainVolume);
        uiVolumeSlider.SetValue (data.uiVolume);
        sfxVolumeSlider.SetValue (data.sfxVolume);
        musicVolumeSlider.SetValue (data.musicVolume);

        onOptionsLoaded.Invoke (data);
    }

    public void ApplyOptions ( )
    {
        SetOptionsDirty (false);

        OptionsData data = OptionsData.Saved;

        //Screen Settings
        data.resolutionIndex = resolutionSelector.CurrentIndex;
        data.frameRateIndex = frameRateSelector.CurrentIndex;
        data.fullscreenMode = fullscreenToggle.IsOn;

        //Graphic Settings
        data.gamma = gammaSlider.Value;
        data.VolumetricClouds = cloudsToggle.IsOn;

        //Audio Settings
        data.mainVolume = musicVolumeSlider.Value;
        data.uiVolume = uiVolumeSlider.Value;
        data.sfxVolume = sfxVolumeSlider.Value;
        data.musicVolume = musicVolumeSlider.Value;

        data.Save ( );

        onOptionsApplied.Invoke (data);  
    }

    public void SetOptionsDirty (bool dirty = true)
    {
        m_isDirty = dirty;
        applyButton.SetActive (dirty);
    }

    #endregion
}

