using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public Simulation Simulation;

    public Slider carSlider;
    public Button bSimuler;

    public TextMeshProUGUI bSimulerTxt;
    public TextMeshProUGUI NbGenTxt;
    public TextMeshProUGUI TimerTxt;

    public Slider mutationRateSlider;
    public TextMeshProUGUI mutationRateTxt;

    public void Start()
    {
        UpdateValues();
        UpdateUI();
    }

    public void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateNbCarsTxt();
        UpdateNbGen();
        UpdateTimer();
        UpdateMutationRateTxt();
    }

    public void UpdateValues()
    {
        UpdateNbCars();
        UpdateMutationRate();
    }


    // MODIFICATION DE TEXTES
    public void UpdateTimer()
    {
        TimerTxt.text = $"Temps Restant : {Math.Round(Simulation.SimulationTime - Simulation.currentSimulationTime,2)}s";
    }

    public void UpdateNbGen()
    {
        NbGenTxt.text = $"Génération {Simulation.NumeroGeneration}";
    }

    public void UpdateNbCarsTxt()
    {
        bSimulerTxt.text = "Simuler " + Simulation.nbCars.ToString("F0") + " Voitures";
    }

    public void UpdateMutationRateTxt()
    {
        mutationRateTxt.text = $"{Math.Round(mutationRateSlider.value, 2)}";
    }


    // MODIFICATION DE VALEURS
    public void UpdateNbCars()
    {
        Simulation.nbCars = 10 * Convert.ToInt32(carSlider.value);
    }

    public void UpdateMutationRate()
    {
        foreach (NeuralNetwork nn in Simulation.NeuralNetworks)
        {
            nn.mutationRate = Math.Round(mutationRateSlider.value,2);
        }
    }
}