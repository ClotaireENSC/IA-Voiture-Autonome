using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject CarPrefab;
    private GameObject[] Cars;

    public int nbCars = 50;

    public void Start()
    {
        Cars = new GameObject[nbCars];

        for (int i=0; i<nbCars; i++)
        {
            Cars[i] = Instantiate(CarPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0));
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            DestroyCars();
            Start();
        }
    }

    public void DestroyCars()
    {
        foreach(GameObject c in Cars)
        {
            Destroy(c);
        }
    }
}
