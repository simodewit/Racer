using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;

    public void Start()
    {
        Instantiate(carPrefab, transform.position, transform.rotation);
    }
}
