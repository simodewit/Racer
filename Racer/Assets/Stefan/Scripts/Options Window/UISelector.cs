using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISelector : OptionInputReceiver
{
    [SerializeField]
    private List<string> options = new ( ) { "Option 1", "Option 2", "Option 3" };

    [Header ("References")]
    public TextMeshProUGUI textElement;
    public Image arrowLeft, arrowRight;

    public Color openColor, closedColor;

    [Header ("Settings")]
    public float animationTime;
    public float inputDelay = 0.2f;

    private float m_animationTimer;
    private float m_leftArrowTimer, m_rightArrowTimer;
    private bool m_leftClosed, m_rightClosed;

    [Header ("Events")]
    public UnityEvent<int> onIndexChanged;

    [SerializeField]
    private int m_currentIndex;
    [SerializeField]
    private float m_inputTimer;

    public int CurrentIndex
    {
        get
        {
            return m_currentIndex;
        }
        private set
        {
            int newValue = Mathf.Clamp (value, 0, options.Count - 1);

            if ( newValue == m_currentIndex )
                return;

            m_currentIndex = newValue;

            OptionsManager.Instance.SetOptionsDirty ( );

            OnValueChanged ( );
        }
    }

    private void Start ( )
    {
        OnValueChanged ( );
        UpdateAnimation (1);
    }

    private void Update ( )
    {
        m_animationTimer += Time.deltaTime;
        float progress = Mathf.InverseLerp (0, animationTime, m_animationTimer);

        m_inputTimer -= Time.deltaTime;

        UpdateAnimation (progress);
    }

    private void UpdateAnimation ( float progress )
    {
        //Check if arrows should be closed

        if ( m_leftClosed && CurrentIndex != 0 ) // is closed but should open
        {
            m_leftClosed = false;
            m_leftArrowTimer = 0;
        }
        else if ( !m_leftClosed && CurrentIndex == 0 ) // is open but should close
        {
            m_leftClosed = true;
            m_leftArrowTimer = 0;
        }

        if ( m_rightClosed && CurrentIndex != options.Count - 1 ) // is closed but should open
        {
            m_rightClosed = false;
            m_rightArrowTimer = 0;
        }
        else if ( !m_rightClosed && CurrentIndex == options.Count - 1 ) // is open but should close
        {
            m_rightClosed = true;
            m_rightArrowTimer = 0;
        }

        m_leftArrowTimer += Time.deltaTime;
        m_rightArrowTimer += Time.deltaTime;

        float leftProgress = Mathf.InverseLerp (0, animationTime, m_leftArrowTimer);
        float rightProgress = Mathf.InverseLerp (0, animationTime, m_rightArrowTimer);

        arrowLeft.color = Color.Lerp (m_leftClosed ? openColor : closedColor, m_leftClosed ? closedColor : openColor, leftProgress);
        arrowRight.color = Color.Lerp (m_rightClosed ? openColor : closedColor, m_rightClosed ? closedColor : openColor, rightProgress);
    }

    public override void OnReceiveHorizontalInput ( System.Single input )
    {
        if ( m_inputTimer < 0 )
        {
            m_inputTimer = inputDelay;

            if ( input < 0 ) // go left
            {
                CurrentIndex--;

                return;
            }
            //go right
            CurrentIndex++;
        }

    }


    public void SetIndex ( int value )
    {
        m_currentIndex = Mathf.Clamp (value, 0, options.Count - 1);

        OnValueChanged ( );
    }

    private void OnValueChanged ( )
    {
        m_animationTimer = 0;

        textElement.text = options[CurrentIndex];

        onIndexChanged.Invoke (CurrentIndex);
    }
}
