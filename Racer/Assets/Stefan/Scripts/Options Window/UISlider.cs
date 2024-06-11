using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : OptionInputReceiver
{
    [Header ("References")]
    public Image fill;
    public TextMeshProUGUI valueText;
    public Image border;

    [Header ("Settings")]
    public float minValue = 0;
    public float maxValue = 1;
    public string labelFormat = "F1";

    public float sensitivity = 2;


    [SerializeField]
    private float m_value = 1;

    public float Value
    {
        get
        {
            return m_value;
        }
        private set
        {
            m_value = Mathf.Clamp(value,minValue,maxValue);

            OnValueChanged ( );
        }
    }

    private void Start ( )
    {
        OnValueChanged ( );
    }

    void OnValueChanged ( )
    {
        UpdateFill ( );

        OptionsManager.Instance.SetOptionsDirty ( );
    }

    void UpdateFill ( )
    {
        float progress = Mathf.InverseLerp (minValue, maxValue, Value);

        fill.fillAmount = progress;

        valueText.text = Value.ToString (labelFormat);
    }

    public override void OnReceiveHorizontalInput ( System.Single input )
    {
        if ( input == 0 )
            return;

        float step = maxValue - minValue ;

        float toAdd = step * sensitivity * input * Time.deltaTime;
        Debug.Log ($"Updated slider, added {toAdd}");
        Value += toAdd;
    }

    public void SetValue(float value )
    {
        Value = value;
        OptionsManager.Instance.SetOptionsDirty (false);
    }

}
