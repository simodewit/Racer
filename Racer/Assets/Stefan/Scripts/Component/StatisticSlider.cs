using com.cyborgAssets.inspectorButtonPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticSlider : MonoBehaviour
{
    [Header ("Settings")]
    public float minValue;
    public float maxValue;
    public string labelFormat = "F2";
    public float smoothTime = 0.1f;
    public bool showDifference = true;

    [Header ("Colors")]
    public Color defaultColor;
    public Color positiveColor, negativeColor;

    [Header ("References")]
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI compareText;
    public Image fill, positiveFill, negativeFill;

    // Private Variables
    private float _value;
    private float _compareValue;
    private bool _compare;

    private float _targetFill,_targetNegativeFill,_targetPositiveFill;

    private Vector3 _fillVelocity;

    private void Update ( )
    {
        // Fill
        fill.fillAmount = Mathf.SmoothDamp (fill.fillAmount, _targetFill, ref _fillVelocity.x, smoothTime);

        // Positive
        positiveFill.fillAmount = Mathf.SmoothDamp (positiveFill.fillAmount, _targetPositiveFill, ref _fillVelocity.y, smoothTime);
        
        // Negative
        negativeFill.fillAmount = Mathf.SmoothDamp (negativeFill.fillAmount, _targetNegativeFill, ref _fillVelocity.z, smoothTime);
    }

    private void UpdateSlider ( )
    {
        float lerp = Mathf.InverseLerp (minValue, maxValue, _value);

        if ( _compare )
        {
            float compareLerp = Mathf.InverseLerp (minValue, maxValue, _compareValue);

            if ( _compareValue < _value ) // fill and negative should switch fill amount
            {
                _targetFill = compareLerp;
                _targetNegativeFill = lerp;

               _targetPositiveFill = 0;
            }
            else // positive fill should be the sum of the compare and normal lerp
            {
                _targetPositiveFill = compareLerp;
                _targetFill = lerp;
                _targetNegativeFill = lerp;
            }

            compareText.gameObject.SetActive (true & showDifference);
            compareText.text = _compareValue.ToString (labelFormat);
        }
        else
        {
            _targetFill = lerp;

            compareText.gameObject.SetActive (false);
        }

        valueText.text = _value.ToString (labelFormat);
    }

    [ProButton]
    public void SetValue ( float value, bool compare = true )
    {
        _value = value;
        _compare = compare;

        UpdateSlider ( );
    }

    [ProButton]
    public void SetComparison ( float compare )
    {
        _compare = true;
        _compareValue = compare;

        UpdateSlider ( );
    }

    public void SetValueAndComparison ( float value, float comparison, bool compare = true )
    {
        _value = value;
        _compareValue = comparison;
        _compare = compare;

        UpdateSlider ( );
    }

    [ProButton]
    public void DisableComparison ( )
    {
        _compareValue = 0;
        _compare = false;

        UpdateSlider ( );
    }

}
