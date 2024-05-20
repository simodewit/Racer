using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;

    public GameObject car;

    public void Start()
    {
        car = Instantiate(carPrefab, transform.position, transform.rotation);
    }
}
