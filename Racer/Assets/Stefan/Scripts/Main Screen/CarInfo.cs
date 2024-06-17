using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarInfo : MonoBehaviour
{
    [Header ("References")]
    public CarSelector selector;
    public Image borderImg;
    public Image lineImage;
    public Image colorImg;
    public Image brandBorderImg;
    public Image brandImg;

    public TextMeshProUGUI carBrandText;
    public TextMeshProUGUI carModelText;

    private void Start ( )
    {
        UpdateInfo ();
    }

    public void UpdateInfo ( )
    {
        CarObject car = selector.SelectedCar;

        //Apply Text
        carBrandText.text = car.carBrand;
        carModelText.text = car.carModel;

        //Apply Colors
        brandBorderImg.color = car.carColor;
        borderImg.color = car.carColor;
        colorImg.color = car.carColor;
        lineImage.color = car.carColor;

        //Apply Sprites
        brandImg.sprite = car.brandSprite;
    }
}
