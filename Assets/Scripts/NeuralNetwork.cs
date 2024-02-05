using System;
using System.Collections.Generic;
using System.Linq;

public class NeuralNetwork
{
    public int[] LayersLengths = { 6, 32, 4 };
    public List<Layer> Layers = new List<Layer>();

    public NeuralNetwork()
    {
        for (int i = 0; i < LayersLengths.Length - 1; i++)
        {
            Layers.Add(new Layer(LayersLengths[i], LayersLengths[i + 1]));
        }
    }

    public NeuralNetwork(NeuralNetwork original_NN)
    {
        LayersLengths = original_NN.LayersLengths;

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

    public double[] Brain(double[] inputs)
    {
        if (inputs.Length != Layers[0].NbInputs)
        {
            throw new ArgumentException(
                "The number of inputs doesn't match the number of inputs of the Layer."
            );
        }

        int outputLayerIndex = Layers.Count() - 1;

        Layers[0].Predict(inputs);
        Layers[0].Activation();

        for (int i = 1; i < outputLayerIndex; i++)
        {
            Layers[i].Predict(Layers[i - 1].NodeArray);
            Layers[i].Activation();
        }

        Layers[outputLayerIndex].Predict(Layers[outputLayerIndex - 1].NodeArray);

        Layers[outputLayerIndex].Convert();

        return Layers[outputLayerIndex].NodeArray;
    }

    public override string ToString()
    {
        string outString = "";
        foreach (Layer l in Layers)
        {
            outString += l.ToString() + "\n";
        }
        return outString;
    }
}