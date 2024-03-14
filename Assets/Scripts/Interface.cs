using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public Simulation Simulation;

    public Slider carSlider;
    public Button bSimuler;
    public TextMeshProUGUI bSimulerTxt;

    public void Start()
    {
        bSimulerTxt = bSimuler.GetComponentInChildren<TextMeshProUGUI>();
        UpdateNbCars();
    }

    public void Update()
    {
    }

    public void UpdateNbCars()
    {
        Simulation.nbCars = 10 * Convert.ToInt32(carSlider.value);
        bSimulerTxt.text = "Simuler " + Simulation.nbCars.ToString("F0") + " Voitures";
    }
}
