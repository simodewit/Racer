using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [Header ("References")]
    public Image optionImage;
    public TextMeshProUGUI optionNameText,optionDescriptionText;
    public VerticalLayoutGroup layout;
    public GameObject applyButton;

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

    [SerializeField]
    private int currentOptionIndex;

    float layoutVelocity;

    bool m_isDirty;
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
        CheckScrolling ( );

        HandleInput ( );

        HandleScrollView ( );
    }

    #region Input
    void HandleInput ( )
    {
        UIOption option = allOptions[currentOptionIndex];

        if ( option == null || option.inputReceiver == null )
            return;

        //TODO: Change to new input system
        float horizontalInput = Input.GetAxisRaw ("Horizontal");

        option.inputReceiver.OnReceiveHorizontalInput (horizontalInput);

        //float verticalInput = Input.GetAxisRaw ("Vertical");

        //if(verticalInput != 0 )
        //{
        //    option.inputReceiver.OnReceiveVerticalInput (verticalInput);
        //}

        //TODO: Add controller input support
        if ( Input.GetButtonDown ("Jump"))
        {
            option.inputReceiver.OnReceiveXInput ( );
        }
    }
    void CheckScrolling ( )
    {
        if ( !Input.GetButtonDown ("Vertical"))
            return;

        if ( Input.GetAxisRaw ("Vertical") < 0 ) // scrolling down
        {
            if ( currentOptionIndex < allOptions.Count - 1 )
            {
                SetCurrentOptionIndex (currentOptionIndex + 1);
            }
        }
        else //Scrolling up
        {
            if ( currentOptionIndex > 0)
            {
                SetCurrentOptionIndex (currentOptionIndex - 1);
            }
        }
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

