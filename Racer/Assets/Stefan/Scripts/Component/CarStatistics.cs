using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarStatistics : MonoBehaviour
{
    [Header("Settings")]
    public float textSpeed = 5;

    [Header ("References")]
    public CarSelector selector;

    [Header("Elements")]
    public TextMeshProUGUI brandText;
    public TextMeshProUGUI modelText;
    public Image brandImage;
    public UIGroup selectedObject;

    public StatisticSlider slider1, slider2, slider3, slider4, slider5;

    // Private Variables
    private float _textTimer;

    private void Update ( )
    {
        UpdateText ( );
    }

    private void UpdateText ( )
    {
        if ( selector.ScrollSelectedCar == null ) return;

        _textTimer += textSpeed * Time.deltaTime;

        int lettersToShow = Mathf.CeilToInt (_textTimer);

        string brand = selector.ScrollSelectedCar.carBrand;
        string model = selector.ScrollSelectedCar.carModel;

        brandText.text = brand[..Mathf.Min(lettersToShow,brand.Length)];

        lettersToShow -=brand.Length;
        lettersToShow = Mathf.Max (0, lettersToShow);

        modelText.text = model[..Mathf.Min(lettersToShow,model.Length)];
    }

    public void UpdateStatistics ( )
    {
        UpdateText ( );

        if ( selector.SelectedCar == null ) return;

        if(selector.ScrollSelectedCar != null )
        {
            slider1.SetValueAndComparison (selector.SelectedCar.value1, selector.ScrollSelectedCar.value1, true);
            slider2.SetValueAndComparison (selector.SelectedCar.value2, selector.ScrollSelectedCar.value2, true);
            slider3.SetValueAndComparison (selector.SelectedCar.value3, selector.ScrollSelectedCar.value3, true);
            slider4.SetValueAndComparison (selector.SelectedCar.value4, selector.ScrollSelectedCar.value4, true);
            slider5.SetValueAndComparison (selector.SelectedCar.value5, selector.ScrollSelectedCar.value5, true);
            
            brandImage.sprite = selector.ScrollSelectedCar.brandSprite;
        }
        else
        {
            slider1.SetValue(selector.SelectedCar.value1, false);
            slider2.SetValue(selector.SelectedCar.value2, false);
            slider3.SetValue(selector.SelectedCar.value3, false);
            slider4.SetValue(selector.SelectedCar.value4, false);
            slider5.SetValue(selector.SelectedCar.value5, false);

            brandImage.sprite = selector.SelectedCar.brandSprite;
        }

        selectedObject.Toggle (selector.SelectedCar != selector.ScrollSelectedCar);

    }

    public void ResetText ( )
    {
        _textTimer = 0;
    }
}
