﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class Abilities : MonoBehaviour
{
    public List<Chart> charts = new List<Chart>(); // population, fitness, speed, size of sensors, energy needed

    public int populationSize = 0;
    public List<Blob> population = new List<Blob>();

    private string targetDNA = "111111111111111111111111";
    private string validCharacters = "01";

    private int numGeneration = 0;
    private float bestFitness = 0;
    private char[] bestGenes = new char[24];
    List<float> allFitness = new List<float>();
    public List<float> allSpeed = new List<float>();
    public List<float> allSize = new List<float>();
    public List<float> allEnergy = new List<float>();

    public List<Blob> parents = new List<Blob>();

    [SerializeField]
    GameObject newBlob;
    [SerializeField]
    List<GameObject> fruits = new List<GameObject>();
    [SerializeField]
    float mutationRate = .1f;

    Transform worldTransform;

    [SerializeField]
    int initialPopulation = 30;
    [SerializeField]
    GameObject blobRoot;
    [SerializeField]
    GameObject fruitRoot;

    int nbOfFruitsToSpawn = 7;
    [SerializeField]
    Slider sliderFruits;
    [SerializeField]
    Slider sliderPopulation;
    [SerializeField]
    TextMeshProUGUI textSliderFruits;
    [SerializeField]
    TextMeshProUGUI textSliderPopulation;

    [SerializeField]
    CinemachineVirtualCamera vCam;
    [SerializeField]
    GameObject smoke;

    public void ChangeNbOfFruitsToSpawn()
    {
        nbOfFruitsToSpawn = Mathf.RoundToInt(sliderFruits.value);
        textSliderFruits.text = nbOfFruitsToSpawn.ToString();
    }
    public void ChangeInitialPopulation()
    {
        initialPopulation = Mathf.RoundToInt(sliderPopulation.value);
        textSliderPopulation.text = initialPopulation.ToString();
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public void AddParents(Blob blob1, Blob blob2)
    {
        if (!parents.Contains(blob1) && !parents.Contains(blob2))
        {
            parents.Add(blob1);
            parents.Add(blob2);

            StartCoroutine(MakingBabies(blob1, blob2));
        }
    }

    IEnumerator MakingBabies(Blob blob1, Blob blob2)
    {
        Vector3 point;
        if (!RandomPoint(blob1.transform.position, 10f, out point))
        {
            point = blob1.transform.position;
        }
        Debug.Log("Make a baby");
        GameObject tmpBlob = Instantiate(newBlob, point, Quaternion.identity);

        GameObject smokePuff = Instantiate(smoke, point, transform.rotation) as GameObject;
        ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
        float totalDuration = parts.duration;
        Destroy(smokePuff, totalDuration);

        tmpBlob.GetComponent<Blob>().Crossover(blob1,blob2);
        tmpBlob.GetComponent<Blob>().Mutate(mutationRate);
        tmpBlob.GetComponent<Blob>().ChangeAppearance();

        yield return new WaitForSeconds(1f);

        if(blob1 != null)
        {
            blob1.energy -= blob1.gene_energyNeeds;
            blob1.agent.SetDestination(blob1.agent.transform.position);
        }
        if(blob2 != null)
        {
            blob2.energy -= blob2.gene_energyNeeds;
            blob2.agent.SetDestination(blob2.agent.transform.position);
        }

        parents.Remove(blob1);
        parents.Remove(blob2);
    }

    //private GeneticAlgorithm ga;

    void Start()
    {
        worldTransform = GameObject.Find("Terrain").transform;
        ChangeInitialPopulation();
        ChangeNbOfFruitsToSpawn();
    }

    public void StartGame()
    {
        newBlob.GetComponent<Blob>().firstGen = true;
        for (int i = 0; i < initialPopulation; ++i)
        {
            float randomX = Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
            float randomZ = Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);

            Vector3 point;
            if (RandomPoint(new Vector3(randomX, 0, randomZ), 30f, out point))
            {
                Instantiate(newBlob, point, Quaternion.identity, blobRoot.transform);
                GameObject smokePuff = Instantiate(smoke, point, transform.rotation) as GameObject;
                ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
                float totalDuration = parts.duration;
                Destroy(smokePuff, totalDuration);
            }
        }
        newBlob.GetComponent<Blob>().firstGen = false;

        InvokeRepeating("SpawnFruit", 0f, 1f);
        InvokeRepeating("UpdateCharts", 0f, 5f);

        //Time.timeScale = 50;
    }

    public void UpdateCharts()
    {
        // population, fitness, speed, size of sensors, energy needed
        charts[0].points.Add(population.Count);
        charts[1].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().fitness) / population.Count : 0.5f);
        charts[2].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_speed) / population.Count : 0.5f);
        charts[3].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_size) / population.Count : 0.5f);
        charts[4].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_energyNeeds) / population.Count : 0.5f);

        foreach (Chart chart in charts)
        {
            chart.UpdatePoints();

        }
        

    }

    void SpawnFruit()
    {
        for(int i=0; i<nbOfFruitsToSpawn; ++i)
        {
            float randomX = Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
            float randomZ = Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);

            Vector3 point;
            if (RandomPoint(new Vector3(randomX, 0, randomZ), 100f, out point))
            {
                Instantiate(fruits[Random.Range(0, fruits.Count-1)], new Vector3(point.x, 30f, point.z), Quaternion.identity, fruitRoot.transform);
            }
        }        
    }

    public char GetRandomGene()
    {
        int i = (int)UnityEngine.Random.Range(0, 2);
        //Debug.Log("Valeur tirée par random.Next : " + i);
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

        allFitness.Add(fitness);

        return fitness;
    }

    private void UpdateText(char[] bestGenes, float bestFitness, int numGeneration, int populationSize)
    {
        Debug.Log("Generation number : " + numGeneration + " Best Genes : " + new string(bestGenes) + " Best Fitness : " + bestFitness + ", Population Size : " + populationSize);
    }
}