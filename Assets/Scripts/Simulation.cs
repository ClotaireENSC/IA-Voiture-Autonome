using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
Classe principale gerant la simulation
(Lancement de n voitures, gestion des réseaux de neurones, algorithme genetique, etc)

RDN : Reseau de Neurones
*/
public class Simulation : MonoBehaviour
{
    public bool Simulating = false;

    public GameObject CarPrefab;

    // Tableaux des Voitures et des RDN en cours
    public NeuralNetwork[] NeuralNetworks = new NeuralNetwork[1];
    public GameObject[] CarsInstances = new GameObject[1];

    // Temps de simulation (1min)
    public double SimulationTime = 60;
    public double currentSimulationTime;

    public int nbCars;

    public GameObject Camera;

    public int NumeroGeneration = 0;

    public void Init()
    {
        NeuralNetworks = new NeuralNetwork[nbCars];
        CarsInstances = new GameObject[nbCars];
    }


    // Lancement d'une simulation avec des voitures a RDN aleatoires
    public void SimulateRandom()
    {
        DestroyCars();
        Init();

        NumeroGeneration = 0;

        // Remplissage des RDN
        for (int i = 0; i < nbCars; i++)
        {
            NeuralNetworks[i] = new NeuralNetwork();
            NeuralNetworks[i].Randomize();
        }

        StartSimulation();
    }


    // A chaque frame, si toutes les voitures sont arretees, on passe a la prochaine simulation
    public void Update()
    {
        if (Simulating)
        {
            if (currentSimulationTime > SimulationTime || AllCarStopped())
            {
                GoNextGeneration();
            }

            currentSimulationTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector3(-107.875f, 2f, 657.49f);
        }
    }


    // Lance une simulation
    public void StartSimulation()
    {
        InstantiateCars(NeuralNetworks);
        currentSimulationTime = 0;
        ResetCamera();
        Simulating = true;
    }


    // Cree les voitures dans l'environnement avec leurs RDN respectifs
    public void InstantiateCars(NeuralNetwork[] neuralNetworks)
    {
        CarsInstances = new GameObject[neuralNetworks.Length];
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            CarsInstances[i] = Instantiate(CarPrefab, transform.position, transform.rotation);
            CarsInstances[i].GetComponent<CarController>().NeuralNetwork = neuralNetworks[i];
        }
    }


    // Passage d'une simulation à la suivante
    public void GoNextGeneration()
    {
        // Tri des RDN pour avoir les plus performants
        SortNeuralNetworks();

        // Sauvegarde du meilleur RDN dans un .txt
        SaveBestNN();

        // Creation de nouveaux RDN (par mutation des meilleurs)
        GenerateNewNeuralNetworks();

        // Suppression des instances des voitures dans l'environnement
        DestroyCars();

        // Debut de la nouvelle simulation
        StartSimulation();

        NumeroGeneration++;
    }


    // Tri des meilleurs RDN (10%)
    public void SortNeuralNetworks()
    {
        // Tri a bulle classique sur les RDN

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


    // Ecriture du meilleur RDN dans un .txt
    public void SaveBestNN()
    {
        string filePath = "BestNeuralNetworks.txt";
        string content = NeuralNetworks[0].ToString();

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.Write(content);
        }
    }


    // Mutation des 10% meilleurs RDN pour reccrer un echantillon de "nbCars" RDN
    public void GenerateNewNeuralNetworks()
    {
        NeuralNetwork[] NewNeuralNetworks = new NeuralNetwork[nbCars];

        int NbDixieme = nbCars / 10;


        // Creation du nouvel echantillon avec des RDN identiques
        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j <= NbDixieme; j++)
            {
                NewNeuralNetworks[NbDixieme * i + j - 1] = new NeuralNetwork(NeuralNetworks[i]);
            }
        }


        // Mutations
        for (int i = 0; i < nbCars; i++)
        {
            NewNeuralNetworks[i].Mutate();
        }

        NeuralNetworks = NewNeuralNetworks;
    }


    // Suppression des voitures dans l'environnement
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


    // Recherche de si toutes les voitures se sont arretees
    public bool AllCarStopped()
    {
        if (CarsInstances[0] != null)
        {
            foreach (GameObject c in CarsInstances)
            {
                if (c.GetComponent<CarController>().currentSpeed > 0.09f && c.GetComponent<CarController>().score > 0)
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


    // Charge le dernier RDN enregistre dans "BestNeuralNetworks.txt"
    public NeuralNetwork LoadBestNeuralNetwork()
    {
        string line;

        StreamReader sr = new StreamReader("BestNeuralNetworks.txt");
        line = sr.ReadLine()!;

        string[] shapeList = line.Split(" ");
        int[] shapeInt = new int[shapeList.Length];

        for (int i = 0; i < shapeList.Length - 1; i++) shapeInt[i] = Convert.ToInt32(shapeList[i]);

        NeuralNetwork NN = new NeuralNetwork();

        for (int i = 1; i < shapeInt.Length; i++)
        {
            int actualNbNeurons = shapeInt[i];
            int actualNbInputs = shapeInt[i - 1];

            for (int j = 0; j < actualNbInputs; j++)
            {
                for (int k = 0; k < actualNbNeurons; k++)
                {
                    NN.Layers[i - 1].WeightArray[j, k] = (float)Convert.ToDouble(sr.ReadLine());
                }
            }

            for (int k = 0; k < actualNbNeurons; k++)
            {
                NN.Layers[i - 1].BiasArray[k] = (float)Convert.ToDouble(sr.ReadLine());
            }
        }
        return NN;
    }


    // Lance la simulation de la meilleure voiture enregistrée a ce moment
    public void SimulateBestCar()
    {
        NeuralNetwork nn = LoadBestNeuralNetwork();

        NeuralNetworks = new NeuralNetwork[] { nn };

        // Suppression des instances des voitures dans l'environnement
        DestroyCars();

        // Debut de la nouvelle simulation
        StartSimulation();
    }
}
