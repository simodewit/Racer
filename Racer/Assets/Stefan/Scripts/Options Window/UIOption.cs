using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    [Header ("Option Data")]
    public string optionName = "Option";
    [TextArea(3,5)]
    public string optionDescription = "Description";
    public Sprite optionSprite;

    [Header ("References")]
    public Image background;
    public Image border, lines;
    public TextMeshProUGUI labelText;

    public OptionInputReceiver inputReceiver;

    [Header("Animation")]
    public Color selectedBGColor;
    public Color unselectedBGColor;

    public Color selectedBorderColor;
    public Color unselectedBorderColor;

    public Color selectedLinesColor;
    public Color unselectedLinesColor;


    public float lerpTime;

    private float _lerpTimer;

    private bool m_selected;

    private bool shouldUpdate;

    private void Start ( )
    {
        _lerpTimer = lerpTime;

    }

    private void OnValidate ( )
    {
        labelText.text = optionName;
    }

    private void Update ( )
    {
        _lerpTimer += Time.deltaTime;

        if ( _lerpTimer < lerpTime )
        {
            UpdateAnimation (CalculateAnimationProgress());
        }
        else if ( shouldUpdate )
        {
            shouldUpdate = false;
            UpdateAnimation (1);
        }
    }

    void UpdateAnimation (float progress)
    {
        background.color = Color.Lerp (m_selected ? unselectedBGColor : selectedBGColor, m_selected ? selectedBGColor : unselectedBGColor, progress);

        border.color = Color.Lerp (m_selected ? unselectedBorderColor : selectedBorderColor, m_selected ? selectedBorderColor : unselectedBorderColor, progress);

        lines.color = Color.Lerp (m_selected ? unselectedLinesColor : selectedLinesColor, m_selected ? selectedLinesColor : unselectedLinesColor, progress);

    }

    public void SetSelected(bool selected )
    {
        m_selected = selected;
        _lerpTimer = 0;
        shouldUpdate = true;
    }

    public float CalculateAnimationProgress ( )
    {
        return Mathf.InverseLerp (0, lerpTime, _lerpTimer);
    }
}
