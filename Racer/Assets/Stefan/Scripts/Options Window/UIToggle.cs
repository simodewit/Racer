using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : OptionInputReceiver
{
    [SerializeField]
    private bool m_isOn = true;

    [Header ("Settings")]
    public float animationTime;
    public Color activeBackgroundColor;
    public Color inactiveBackgroundColor;

    public Color activeTextColor;
    public Color inactiveTextColor;

    public Color activeLinesColor;
    public Color inactiveLinesColor;

    [Header ("Animation References")]
    public Image onBackground;
    public Image onLines;
    public TextMeshProUGUI onText;

    public Image offBackground;
    public Image offLines;
    public TextMeshProUGUI offText;

    private float m_animationTimer;

    public bool IsOn
    {
        get
        {
            return m_isOn;
        }
        private set
        {
            m_isOn = value;

            OnValueChanged ( );
        }
    }

    private void Start ( )
    {
        UpdateAnimation (1);
    }
    private void Update ( )
    {
        m_animationTimer += Time.deltaTime;

        float progress = Mathf.InverseLerp (0, animationTime, m_animationTimer);
        UpdateAnimation (progress);
    }

    void UpdateAnimation ( float progress )
    {
        onBackground.color = LerpColor (activeBackgroundColor, inactiveLinesColor);
        onLines.color = LerpColor (activeLinesColor, inactiveLinesColor);
        onText.color = LerpColor (activeTextColor, inactiveTextColor);

        offBackground.color = LerpColor (inactiveBackgroundColor, activeBackgroundColor);
        offLines.color = LerpColor (inactiveLinesColor, activeLinesColor);
        offText.color = LerpColor (inactiveTextColor, activeTextColor);

        Color LerpColor ( Color active, Color inactive )
        {
            return Color.Lerp (IsOn ? inactive : active, IsOn ? active : inactive, progress);
        }
    }

    public void SetValue (bool value)
    {
        m_isOn = value;
        m_animationTimer = animationTime;
    }

    private void OnValueChanged ( )
    {
        m_animationTimer = 0;

        OptionsManager.Instance.SetOptionsDirty ( );
    }

    public override void OnReceiveXInput ( )
    {
        IsOn = !IsOn;
    }
}