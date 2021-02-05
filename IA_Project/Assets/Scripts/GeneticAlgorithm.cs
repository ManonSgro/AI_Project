using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
	public float mutationRate = 0.01f;

	private List<Blob> Population;
	private List<Blob> newPopulation;

	public GeneticAlgorithm(int populationSize)
	{
		Population = new List<Blob>(populationSize);
		newPopulation = new List<Blob>(populationSize);

		for (int i = 0; i < populationSize; i++)
		{
			Population.Add(new Blob());
		}
	}

	public void NewGeneration()
	{
		if (Population.Count <= 0)
		{
			Debug.Log("All Blobs are dead... Too bad :'( ");
			return;
		}

		List<Blob> newPopulation = new List<Blob>();

		for (int i = 0; i < Population.Count; i++)
		{
			Blob parent1 = ChooseParent();
			Blob parent2 = ChooseParent();

			Blob child = Blob.Crossover(parent1, parent2);

			child.Mutate(mutationRate);

			newPopulation.Add(child);
		}

		Population = newPopulation;
		Debug.Log("New generation generated");

	}

	private Blob ChooseParent()
	{
		int i = (int)UnityEngine.Random.Range(0, Population.Count);

		return Population[i];

	}
}