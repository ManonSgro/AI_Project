using UnityEngine;
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

    private string targetDNA = "1111111111111111111111111111111111111111";
    private string validCharacters = "01";

    private float bestFitness = 0;

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
    public CinemachineVirtualCamera vCam;

    [SerializeField]
    public Transform safePos;

    bool gameInactive = true;

    [SerializeField]
    public BlobPanel blobPanel;

    // Options before game
    public bool sensorsError = false;
    public bool speedAffectsEnergy = true;
    public bool sensorsAffectsEnergy = true;
    public bool energyAffectsBabies = true;
    public bool canFormGroups = true;
    public bool canShareEnergy = true;
    [SerializeField]
    SwitchButton sensorsErrorInput;
    [SerializeField]
    SwitchButton speedAffectEngergyInput;
    [SerializeField]
    SwitchButton sensorsAffectsEnergyInput;
    [SerializeField]
    SwitchButton energyAffectsBabiesInput;
    [SerializeField]
    SwitchButton canFormGroupsInput;
    [SerializeField]
    SwitchButton canShareEnergyInput;

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
        for (int i = 0; i < 10; i++)
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
        yield return new WaitForSeconds(2f);
        Vector3 pos = safePos.position;
        if (blob1 != null)
        {
            pos = blob1.transform.position;
        }
        if (blob2 != null)
        {
            pos = blob2.transform.position;
        }
        Vector3 point;
        if (!RandomPoint(pos, 100f, out point))
        {
            point = pos;
        }
        GameObject tmpBlob = Instantiate(newBlob, point, Quaternion.identity);

        //PlayParticles(point);

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
        population.Clear();
        ClearCharts();

        gameInactive = false;

        sensorsAffectsEnergy = sensorsErrorInput.isSelected;
        speedAffectsEnergy = speedAffectEngergyInput.isSelected;
        sensorsAffectsEnergy = sensorsAffectsEnergyInput.isSelected;
        energyAffectsBabies = energyAffectsBabiesInput.isSelected;
        canFormGroups = canFormGroupsInput.isSelected;
        canShareEnergy = canShareEnergyInput.isSelected;

        newBlob.GetComponent<Blob>().firstGen = true;
        for (int i = 0; i < initialPopulation; ++i)
        {
            float randomX = Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
            float randomZ = Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);

            Vector3 point;
            if (RandomPoint(new Vector3(randomX, 0, randomZ), 30f, out point))
            {
                var tmpBlob = Instantiate(newBlob, point, Quaternion.identity, blobRoot.transform);
                //tmpBlob.transform.position = point;
            }
        }
        newBlob.GetComponent<Blob>().firstGen = false;

        InvokeRepeating("SpawnFruit", 0f, 1f);
        InvokeRepeating("UpdateCharts", 0f, 5f);

        //Time.timeScale = 50;
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;

        CancelInvoke("SpawnFruit");
        CancelInvoke("UpdateCharts");

        gameInactive = true;
        UpdateCharts();
    }
    public void RestartGame()
    {
        gameInactive = false;
        Time.timeScale = 1f;

        InvokeRepeating("SpawnFruit", 0f, 1f);
        InvokeRepeating("UpdateCharts", 0f, 5f);
    }

    public void StopGame()
    {
        Time.timeScale = 1f;

        var children = new List<GameObject>();
        foreach (Transform child in blobRoot.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        var fruitsChildren = new List<GameObject>();
        foreach (Transform child in fruitRoot.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        CancelInvoke("SpawnFruit");
        CancelInvoke("UpdateCharts");
        gameInactive = true;
        UpdateCharts();
    }

    public void UpdateCharts()
    {
        // population, fitness, speed, size of sensors, energy needed
        charts[0].points.Add(population.Count);
        charts[1].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().fitness) / population.Count : 0.5f);
        charts[2].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_speed) / population.Count : 0.5f);
        charts[3].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_size) / population.Count : 0.5f);
        charts[4].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_energyNeeds) / population.Count : 0.5f);
        charts[5].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().group.members.Count) / population.Count : 0.5f);
        charts[6].points.Add(population.Count > 0 ? population.Sum(e => e.GetComponent<Blob>().gene_share) / population.Count : 0.5f);

        foreach (Chart chart in charts)
        {
            chart.UpdatePoints(!gameInactive);

        }
        

    }

    public void ClearCharts()
    {
        foreach (Chart chart in charts)
        {
            chart.points.Clear();

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
        int i = Random.Range(0, 2);
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
            //bestGenes = genes;
        }

        return fitness;
    }
}
