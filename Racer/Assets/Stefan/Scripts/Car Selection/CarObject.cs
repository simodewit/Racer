using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Car Object", menuName = "Car Objects/Car")]
public class CarObject : ScriptableObject
{
    [Header("Car Info")]
    public string carModel;
    public string carBrand;
    public Sprite brandSprite, carImage;
    public Color carColor;

    public bool unlocked;

    [Header ("References")]
    public GameObject carPrefab;
    public GameObject showcasePrefab;

    [Header ("Car Statistics")]
    public float value1;
    public float value2, value3, value4, value5;

    public string FullName
    {
        get
        {
            return carBrand + " " + carModel;
        }
    }
}
