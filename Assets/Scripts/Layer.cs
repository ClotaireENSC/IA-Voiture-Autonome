using System;

public class Layer
{
    public int NbInputs { get; private set; }
    public int NbNodes { get; private set; }

    public double[,] WeightArray;
    public double[] BiasArray;
    public double[] NodeArray;

    public Layer(int nbInputs, int nbNodes)
    {
        NbInputs = nbInputs;
        NbNodes = nbNodes;

        WeightArray = new double[nbInputs, nbNodes];
        NodeArray = new double[nbNodes];
        BiasArray = new double[nbNodes];

        Init();
    }

    public void Init()
    {
        Random rnd = new Random();

        // Remplir WeightArray avec des valeurs aléatoires entre -1 et 1
        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                WeightArray[i, j] = rnd.NextDouble() * 2 - 1;
            }
        }

        // Remplir BiasArray avec des valeurs aléatoires entre 1 et 10
        for (int i = 0; i < BiasArray.Length; i++)
        {
            //BiasArray[i] = rnd.NextDouble() * 9 + 1;
            BiasArray[i] = rnd.NextDouble() * 2 -1;
        }
    }

    public void Predict(double[] inputArray)
    {
        if (inputArray.Length != NbInputs)
        {
            throw new ArgumentException(
                "The number of inputs doesn't match the number of inputs of the Layer."
            );
        }

        for (int i = 0; i < NbNodes; i++)
            NodeArray[i] = 0;

        for (int i = 0; i < NbNodes; i++)
        {
            for (int j = 0; j < NbInputs; j++)
            {
                NodeArray[i] += WeightArray[j, i] * inputArray[j];
            }
            NodeArray[i] += BiasArray[i];
        }
    }

    public void Activation()
    {
        for (int i = 0; i < NbNodes; i++)
        {
            if (NodeArray[i] < 0)
            {
                NodeArray[i] = 0;
            }
        }
    }

    public void Convert()
    {
        for (int i = 0; i < NbNodes; i++)
        {
            if (NodeArray[i] < 0)
            {
                NodeArray[i] = 0;
            }
            else
            {
                NodeArray[i] = 1;
            }
        }
    }

    public override string ToString()
    {
        string outString = "===\nInput vers Layer :\n";

        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                outString += $"\tInput.{i} -> Layer.{j} : {WeightArray[i, j]}\n";
            }
        }

        outString += "Biais :\n";

        for (int i = 0; i < BiasArray.Length; i++)
        {
            outString += $"\tLayer.{i} : {BiasArray[i]}\n";
        }

        outString += "===";

        return outString;
    }
}