using UnityEngine;

public class Abilities : MonoBehaviour
{
    public int populationSize = 12;

    private string targetDNA = "111111111111111111111111";
    private string validCharacters = "01";

    private int numGeneration = 0;
    private float bestFitness = 0;
    private char[] bestGenes = new char[24];

    //private GeneticAlgorithm ga;

    void Start()
    {
        Debug.Log("Start Genetic Algorithm");
        //ga = new GeneticAlgorithm(populationSize);
    }

    void Update()
    {
        /*
        ga.NewGeneration();

        numGeneration++;

        UpdateText(bestGenes, bestFitness, numGeneration, populationSize);

        if (numGeneration == 10)
        {
            Debug.Log("End");
            this.enabled = false;
        }
        */
    }

    public char GetRandomGene()
    {
        int i = (int)UnityEngine.Random.Range(0, 2);
        Debug.Log("Valeur tirée par random.Next : " + i);
        return validCharacters[i];
    }

    public float calculateFitness(char[] genes)
    {
        float fitness = 0;

        for (int i = 0; i < genes.Length; i++)
        {
            if (genes[i] == targetDNA[i])
            {
                fitness += 1;
            }
        }

        fitness /= targetDNA.Length;

        fitness = (Mathf.Pow(2, fitness) - 1) / (2 - 1);
        Debug.Log("Fitness sur 1 : " + fitness);

        if (fitness > bestFitness)
        {
            bestFitness = fitness;
            bestGenes = genes;
        }

        return fitness;
    }

    private void UpdateText(char[] bestGenes, float bestFitness, int numGeneration, int populationSize)
    {
        Debug.Log("Generation number : " + numGeneration + " Best Genes : " + new string(bestGenes) + " Best Fitness : " + bestFitness + ", Population Size : " + populationSize);
    }
}
