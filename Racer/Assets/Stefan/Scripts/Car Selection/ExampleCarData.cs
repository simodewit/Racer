using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExampleCarData 
{
    public string carName;
    public string brandName;
    public Sprite brandLogo;
    public Sprite carImage;
    public Color carColor;

    public bool unlocked;

    public Stats stats;
    public string FullName
    {
        get
        {
            return brandName + " " + carName;
        }
    }

    [System.Serializable]
    public struct Stats
    {
        public float value1, value2, value3, value4;
    }
}
