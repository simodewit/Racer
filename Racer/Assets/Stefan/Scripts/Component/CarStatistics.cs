using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarStatistics : MonoBehaviour
{
    [Header ("References")]
    public CarSelector selector;

    [Header("Elements")]
    public TextMeshProUGUI brandText;
    public TextMeshProUGUI modelText;

    public StatisticSlider slider1, slider2, slider3, slider4, slider5;

    // Private Variables

    public void UpdateStatistics ( )
    {
        if ( selector.SelectedCar == null ) return;

        if(selector.ScrollSelectedCar != null )
        {
            slider1.SetValueAndComparison (selector.SelectedCar.value1, selector.ScrollSelectedCar.value1, true);
            slider2.SetValueAndComparison (selector.SelectedCar.value2, selector.ScrollSelectedCar.value2, true);
            slider3.SetValueAndComparison (selector.SelectedCar.value3, selector.ScrollSelectedCar.value3, true);
            slider4.SetValueAndComparison (selector.SelectedCar.value4, selector.ScrollSelectedCar.value4, true);
            slider5.SetValueAndComparison (selector.SelectedCar.value5, selector.ScrollSelectedCar.value5, true);

            brandText.text = selector.ScrollSelectedCar.carBrand;
            modelText.text = selector.ScrollSelectedCar.carModel;
        }
        else
        {
            slider1.SetValue(selector.SelectedCar.value1, false);
            slider2.SetValue(selector.SelectedCar.value2, false);
            slider3.SetValue(selector.SelectedCar.value3, false);
            slider4.SetValue(selector.SelectedCar.value4, false);
            slider5.SetValue(selector.SelectedCar.value5, false);

            brandText.text = selector.SelectedCar.carBrand;
            modelText.text = selector.SelectedCar.carModel;
        }
    }
}
