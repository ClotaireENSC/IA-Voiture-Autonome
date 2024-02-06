using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject CarPrefab;
    private GameObject[] Cars;
    private GameObject[] CarsInstances;

    private double SimulationTime;

    public int nbCars = 50;

    public void Start()
    {
        Cars = new GameObject[nbCars];
        CarsInstances = new GameObject[nbCars];
    }

    public void StartSimulation()
    {
        for (int i =0; i<nbCars;i++)
        {
            Cars[i] = CarPrefab;
        }

        InstantiateCars();
        SimulationTime = 0;
    }

    public void Simulate()
    {
        InstantiateCars();
        SimulationTime = 0;
    }

    public void InstantiateCars()
    {
        for (int i = 0; i < nbCars; i++)
        {
            CarsInstances[i] = Instantiate(Cars[i], new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0));
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            DestroyCars();
            StartSimulation();
        }

        if (SimulationTime > 15)
        {
            GoNextGeneration();
        }

        SimulationTime += Time.deltaTime;
    }

    public void GoNextGeneration()
    {
        MutateCars();
        Simulate();
    }

    public void MutateCars()
    {
        GameObject[] NewCars = new GameObject[nbCars];

        // Ajouter les meilleures voitures à la liste et les dupliquer

        foreach(GameObject c in Cars)
        {
            CarController carController = c.GetComponent<CarController>();
            carController.NeuralNetwork.Mutate();
        }
    }

    public void DestroyCars()
    {
        if (Cars[0] != null)
        {
            foreach (GameObject c in CarsInstances)
            {
                Destroy(c);
            }
        }
    }
}
