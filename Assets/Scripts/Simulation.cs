using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject CarPrefab;
    private GameObject[] Cars;
    private GameObject[] CarsInstances;

    private double SimulationTime = 10;
    private double currentSimulationTime;

    public int nbCars = 50;

    public void Start()
    {
        Cars = new GameObject[nbCars];
        CarsInstances = new GameObject[nbCars];
    }

    public void StartSimulation()
    {
        for (int i = 0; i < nbCars; i++)
        {
            Cars[i] = CarPrefab;
        }

        InstantiateCars();
        currentSimulationTime = 0;
    }

    public void Simulate()
    {
        DestroyCars();
        InstantiateCars();
        currentSimulationTime = 0;
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

        if (currentSimulationTime > SimulationTime)
        {
            GoNextGeneration();
        }

        currentSimulationTime += Time.deltaTime;
    }

    public void GoNextGeneration()
    {
        //SortCars();
        GenerateNewCars();
        Simulate();
    }

    public void SortCars()
    {
        // Créer une liste temporaire pour stocker les paires (GameObject, score)
        List<KeyValuePair<GameObject, int>> carScores = new List<KeyValuePair<GameObject, int>>();

        // Remplir la liste temporaire avec les voitures et leurs scores
        for (int i = 0; i < nbCars; i++)
        {
            carScores.Add(new KeyValuePair<GameObject, int>(Cars[i], Cars[i].GetComponent<CarController>().score));
        }

        // Trier la liste en fonction des scores (ordre décroissant)
        carScores.Sort((x, y) => x.Value.CompareTo(y.Value));

        // Mettre à jour la liste des voitures dans l'ordre trié
        for (int i = 0; i < nbCars; i++)
        {
            Cars[i] = carScores[i].Key;
        }
    }


    public void GenerateNewCars()
    {
        //CarController carController;
        GameObject[] NewCars = new GameObject[nbCars];

        for (int j = 1; j <= nbCars / 10; j++)
        {
            for (int k = 0; k < 10; k++)
            {
                NewCars[j + 10 * k - 1] = Cars[0];
            }
        }

        //foreach (GameObject c in NewCars)
        //{
        //    carController = c.GetComponent<CarController>();
        //}

        Cars = NewCars;
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
