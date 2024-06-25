using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("The car that should be spawned")]
    [SerializeField] private GameObject carPrefab;
    [HideInInspector] public GameObject car;

    public void Start()
    {
        car = Instantiate(carPrefab, transform.position, transform.rotation);
    }
}
