using System;

/*
Cette classe représente la couche d'un réseau de neurones artificiels (RNA) avec plusieurs couches.

Une couche est definie par le nombre d'entrees, sont nombre de neurones, les poids et les biais des liens/neurones

RDN : Reseau de Neurones
*/
public class Layer
{
    public int NbInputs { get; private set; }
    public int NbNodes { get; private set; }

    public double[,] WeightArray;
    public double[] BiasArray;

    // NodeArray est la valeur finale des neurones de la couche (different des biais)
    public double[] NodeArray;


    // Constructeur
    public Layer(int nbInputs, int nbNodes)
    {
        NbInputs = nbInputs;
        NbNodes = nbNodes;

        WeightArray = new double[nbInputs, nbNodes];
        NodeArray = new double[nbNodes];
        BiasArray = new double[nbNodes];

        Init();
    }


    // Remplissage des differents tableaux
    public void Init()
    {
        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                WeightArray[i, j] = 0;
            }
        }

        for (int i = 0; i < BiasArray.Length; i++)
        {
            BiasArray[i] = 0;
        }
    }


    // Remplissage des tableaux avec des valeurs aleatoires
    public void Randomize()
    {
        Random rnd = new Random();

        // Remplir WeightArray avec des valeurs aleatoires entre -1 et 1
        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                WeightArray[i, j] = rnd.NextDouble() * 2 - 1;
            }
        }

        // Remplir BiasArray avec des valeurs aleatoires entre -1 et 1
        for (int i = 0; i < BiasArray.Length; i++)
        {
            BiasArray[i] = rnd.NextDouble() * 2 - 1;
        }
    }


    // Calcul des valeurs de sortie des neurones de la couche selon les valeurs d'entree (inputArray)
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


    // Application de la fonction d'activation (ici on utilise la rampe)
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


    // Fonction "d'activation" utilisee pour la derniere couche, afin que les valeurs soient 0 ou 1
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


    // Applique la mutation a la couche selon un taux et une taille de mutation
    public void Mutate(double mutationRate, double mutationRange)
    {
        Random rand = new Random();

        // Mutate weights
        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                if (rand.NextDouble() < mutationRate)
                {
                    // Add random noise within mutationRange to the weight
                    WeightArray[i, j] += (2 * rand.NextDouble() - 1) * mutationRange;
                }
            }
        }

        // Mutate biases
        for (int i = 0; i < BiasArray.Length; i++)
        {
            if (rand.NextDouble() < mutationRate)
            {
                // Add random noise within mutationRange to the bias
                BiasArray[i] += (2 * rand.NextDouble() - 1) * mutationRange;
            }
        }
    }

    public override string ToString()
    {
        string outString = "";

        for (int i = 0; i < WeightArray.GetLength(0); i++)
        {
            for (int j = 0; j < WeightArray.GetLength(1); j++)
            {
                outString += $"{WeightArray[i, j]}\n";
            }
        }

        for (int i = 0; i < BiasArray.Length; i++)
        {
            outString += $"{BiasArray[i]}\n";
        }

        return outString;
    }
}