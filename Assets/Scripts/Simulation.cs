using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject CarPrefab;
    private NeuralNetwork[] NeuralNetworks;
    private GameObject[] CarsInstances;

    private double SimulationTime = 60;
    private double currentSimulationTime;

    public int nbCars = 50;

    public void Start()
    {
        NeuralNetworks = new NeuralNetwork[nbCars];
        CarsInstances = new GameObject[nbCars];

        for (int i = 0; i < nbCars; i++)
        {
            NeuralNetworks[i] = new NeuralNetwork();
            NeuralNetworks[i].Randomize();
        }
    }

    public void StartSimulation()
    {
        InstantiateCars(NeuralNetworks);
        currentSimulationTime = 0;
    }

    public void InstantiateCars(NeuralNetwork[] neuralNetworks)
    {
        for (int i = 0; i < nbCars; i++)
        {
            CarsInstances[i] = Instantiate(CarPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0));
            CarsInstances[i].GetComponent<CarController>().NeuralNetwork = neuralNetworks[i];
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            DestroyCars();
            StartSimulation();
        }

        if (currentSimulationTime > SimulationTime || AllCarCrashed() || Input.GetKey(KeyCode.RightArrow))
        {
            GoNextGeneration();
        }

        currentSimulationTime += Time.deltaTime;
    }

    public void GoNextGeneration()
    {
        SortNeuralNetworks();
        GenerateNewNeuralNetworks();
        DestroyCars();
        StartSimulation();
    }

    public void SortNeuralNetworks()
    {
        for (int i = 0; i < nbCars - 1; i++)
        {
            for (int j = 0; j < nbCars - i - 1; j++)
            {
                if (CarsInstances[j].GetComponent<CarController>().score < CarsInstances[j + 1].GetComponent<CarController>().score)
                {
                    // Échange les réseaux de neurones
                    GameObject tempCar = CarsInstances[j];
                    CarsInstances[j] = CarsInstances[j + 1];
                    CarsInstances[j + 1] = tempCar;

                    NeuralNetwork tempNN = NeuralNetworks[j];
                    NeuralNetworks[j] = NeuralNetworks[j + 1];
                    NeuralNetworks[j + 1] = tempNN;
                }
            }
        }
    }

    public void GenerateNewNeuralNetworks()
    {
        NeuralNetwork[] NewNeuralNetworks = new NeuralNetwork[nbCars];

        int NbDixieme = nbCars / 10;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j <= NbDixieme; j++)
            {
                NewNeuralNetworks[NbDixieme * i + j - 1] = new NeuralNetwork(NeuralNetworks[i]);
            }
        }

        for (int i = 0; i < nbCars; i++)
        {
            NewNeuralNetworks[i].Mutate();
        }

        NeuralNetworks = NewNeuralNetworks;
    }

    public void DestroyCars()
    {
        if (CarsInstances[0] != null)
        {
            foreach (GameObject c in CarsInstances)
            {
                Destroy(c);
            }
        }
    }

    public bool AllCarCrashed()
    {
        if (CarsInstances[0] != null)
        {
            foreach (GameObject c in CarsInstances)
            {
                if (!c.GetComponent<CarController>().Collision)
                    return false;
            }
            return true;
        }
        return false;
    }
}
