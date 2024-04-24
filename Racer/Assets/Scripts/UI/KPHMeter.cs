using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KPHMeter : MonoBehaviour
{
    [SerializeField] private CarSpawner spawner;
    [SerializeField] private TextMeshProUGUI textComponent;

    private Speedometer speedometer;

    public void Update()
    {
        //these 2 if statements are in place for potential buggs
        if (speedometer == null && spawner.car != null)
        {
            speedometer = spawner.car.GetComponentInChildren<Speedometer>();
        }

        if(speedometer == null || textComponent == null)
        {
            return;
        }

        int speed = (int)speedometer.speed;
        textComponent.text = speed.ToString();
    }
}
