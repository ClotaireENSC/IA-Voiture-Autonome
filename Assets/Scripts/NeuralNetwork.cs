using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

/*
Cette classe représente un réseau de neurones artificiels (RNA) avec plusieurs couches.

RDN : Reseau de Neurones
*/
public class NeuralNetwork
{
    private static int NextId = 0;
    public int id;

    // Forme du RDN (ici, 6 entrees, 1 couches cachees à 16 neurones, 4 sorties)
    public int[] LayersLengths = { 6, 16, 4 };
    public List<Layer> Layers = new List<Layer>();

    public double mutationRate = 0.2;
    public double mutationRange = 0.5;

    // Constructeur de RDN vide
    public NeuralNetwork()
    {
        id = NextId++;
        for (int i = 0; i < LayersLengths.Length - 1; i++)
        {
            Layers.Add(new Layer(LayersLengths[i], LayersLengths[i + 1]));
        }
    }

    // Constructeur d'un RDN à partir d'un autre RDN (pour eviter les problemes de reference)
    public NeuralNetwork(NeuralNetwork original_NN)
    {
        id = NextId++;
        LayersLengths = original_NN.LayersLengths;
        mutationRate = original_NN.mutationRate;
        mutationRange = original_NN.mutationRange;

        foreach (Layer original_NNLayer in original_NN.Layers)
        {
            Layer newLayer = new Layer(original_NNLayer.NbInputs, original_NNLayer.NbNodes);

            // Copier les poids
            for (int i = 0; i < newLayer.WeightArray.GetLength(0); i++)
            {
                for (int j = 0; j < newLayer.WeightArray.GetLength(1); j++)
                {
                    newLayer.WeightArray[i, j] = original_NNLayer.WeightArray[i, j];
                }
            }

            // Copier les biais
            for (int i = 0; i < newLayer.BiasArray.Length; i++)
            {
                newLayer.BiasArray[i] = original_NNLayer.BiasArray[i];
            }

            Layers.Add(newLayer);
        }
    }


    // Reinitialisation du RDN
    public void RAZ()
    {
        foreach (Layer layer in Layers)
        {
            layer.Init();
        }
    }


    // Fonction principale qui donne des sorties (outputs) a des entrees (inputs)
    public double[] Brain(double[] inputs)
    {
        if (inputs.Length != Layers[0].NbInputs)
        {
            throw new ArgumentException(
                "The number of inputs doesn't match the number of inputs of the Layer."
            );
        }

        int outputLayerIndex = Layers.Count() - 1;


        // Forward propagation
        if (outputLayerIndex != 0)
        {
            Layers[0].Predict(inputs);
            Layers[0].Activation();

            for (int i = 1; i < outputLayerIndex; i++)
            {
                Layers[i].Predict(Layers[i - 1].NodeArray);
                Layers[i].Activation();
            }

            Layers[outputLayerIndex].Predict(Layers[outputLayerIndex - 1].NodeArray);
        }
        else
        {
            Layers[0].Predict(inputs);
        }

        Layers[outputLayerIndex].Convert();

        return Layers[outputLayerIndex].NodeArray;
    }


    // Applique la mutation à chaque couche du RDN
    public void Mutate()
    {
        // mutationRate = 100/(nbGen+200);
        foreach (Layer layer in Layers)
        {
            layer.Mutate(mutationRate, mutationRange);
        }
    }

    public void Randomize()
    {
        foreach (Layer layer in Layers)
        {
            layer.Randomize();
        }
    }

    public override string ToString()
    {
        string outString = "";
        foreach (int i in LayersLengths)
        {
            outString += $"{i} ";
        }
        foreach (Layer l in Layers)
        {
            outString += l.ToString();
        }
        return outString;
    }
}