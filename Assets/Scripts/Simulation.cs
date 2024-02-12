using System;
using System.IO;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject CarPrefab;
    private NeuralNetwork[] NeuralNetworks;
    private GameObject[] CarsInstances;

    private double SimulationTime = 60;
    private double currentSimulationTime;

    public int nbCars = 50;

    public GameObject Camera;

    private int NumeroGeneration = 1;

    public void Start()
    {
        NeuralNetworks = new NeuralNetwork[nbCars];
        CarsInstances = new GameObject[nbCars];

        for (int i = 0; i < nbCars; i++)
        {
            NeuralNetworks[i] = new NeuralNetwork();
            NeuralNetworks[i].Randomize();
        }

        StartSimulation();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.R) && currentSimulationTime > 2)
        {
            NumeroGeneration = 0;
            DestroyCars();
            Start();
        }

        if (currentSimulationTime > SimulationTime || AllCarStopped() || Input.GetKey(KeyCode.RightArrow) && currentSimulationTime > 2)
        {
            GoNextGeneration();
        }

        currentSimulationTime += Time.deltaTime;
    }

    public void StartSimulation()
    {
        InstantiateCars(NeuralNetworks);
        currentSimulationTime = 0;
        ResetCamera();
        Debug.Log($"Génération : {NumeroGeneration++}");
    }

    public void InstantiateCars(NeuralNetwork[] neuralNetworks)
    {
        for (int i = 0; i < nbCars; i++)
        {
            CarsInstances[i] = Instantiate(CarPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0));
            CarsInstances[i].GetComponent<CarController>().NeuralNetwork = neuralNetworks[i];
        }
    }

    public void GoNextGeneration()
    {
        SortNeuralNetworks();
        SaveBestNN();
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

    public void SaveBestNN()
    {
        string filePath = "./Assets/BestNeuralNetworks.txt";
        string content = "\n\n\n\n" + NeuralNetworks[0];

        using (StreamWriter writer = new StreamWriter(filePath,true))
        {
            writer.Write(content);
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
                DestroyImmediate(c);
            }
        }
    }

    public bool AllCarStopped()
    {
        if (CarsInstances[0] != null)
        {
            foreach (GameObject c in CarsInstances)
            {
                if (c.GetComponent<CarController>().currentSpeed > 0.09f && c.GetComponent<CarController>().score>0)
                    return false;
            }
            if (currentSimulationTime > 2)
                return true;
        }
        return false;
    }

    public void ResetCamera()
    {
        Camera.GetComponent<MoveCamera>().Init();
    }
}
