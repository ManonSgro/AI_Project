using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob
{
    private char[] genes;
    private float fitness;
    private static int size = 24;

    public Blob()
    {
        this.genes = new char[size];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = GameObject.FindObjectOfType<Abilities>().GetRandomGene();
            
        }
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = GameObject.FindObjectOfType<Abilities>().calculateFitness(genes);
    }

    public Blob(char[] genes)
    {
        this.genes = new char[size];
        this.genes = genes;
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = GameObject.FindObjectOfType<Abilities>().calculateFitness(genes);
    }

    public static Blob Crossover(Blob parent1, Blob parent2)
    {
        Debug.Log("Start Crossover");
        char[] tempGenes = new char[size];

        for (int i = 0; i < tempGenes.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 1) < 0.5)
            {
                tempGenes[i] = parent1.genes[i];
            }
            else
            {
                tempGenes[i] = parent2.genes[i];
            }
        }
        Debug.Log("Finish Crossover");
        return new Blob(tempGenes);
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 1) < mutationRate)
            {
                genes[i] = GameObject.FindObjectOfType<Abilities>().GetRandomGene();
            }
        }
    }
}
