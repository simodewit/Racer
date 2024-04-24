using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RPMMeter : MonoBehaviour
{
    [SerializeField] private CarSpawner spawner;
    [SerializeField] private TextMeshProUGUI textComponent;

    private Car car;

    public void Update()
    {
        //these 2 if statements are in place for potential buggs
        if (car == null && spawner.car != null)
        {
            car = spawner.car.GetComponentInChildren<Car>();
        }

        if (car == null || textComponent == null)
        {
            return;
        }

        textComponent.text = car.currentRPM.ToString();
    }
}
