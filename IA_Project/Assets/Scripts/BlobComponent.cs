using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlobState { None, Hungry, Fertile, Dead }

public class BlobComponent : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent blob;
    public int energy;
    private BlobState _state;
    private bool seeTarget;
    private GameObject fruitToEat;
    private GameObject target;
    private int frames;

    // Start is called before the first frame update
    void Start()
    {
        _state = BlobState.None;
        seeTarget = false;
        StartCoroutine("UpdateEnergy");
    }

    void UpdateState()
    {
        if (energy < 70) _state = BlobState.Hungry;
        if (energy >= 70) _state = BlobState.Fertile;
        if (energy == 0) _state = BlobState.Dead;
    }

    IEnumerator UpdateEnergy()
    {
        for (; ; )
        {
            // execute block of code here
            if (blob.velocity != new Vector3(0, 0, 0)) energy--;
            yield return new WaitForSeconds(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnergy();
        if (_state == BlobState.None)
        {
            seeTarget = false;
            fruitToEat = null;
            target = null;
            UpdateState();
            switch (_state)
            {
                case BlobState.Hungry:
                    SearchFood();
                    break;
                case BlobState.Fertile:
                    SearchPartner();
                    break;
                case BlobState.Dead:
                    //Die();
                    break;
                default:
                    break;
            }
        }
    }




    void SearchFood()
    {
        //Si l'agent n'a pas de cible, il cherche de la nourriture
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        if (!seeTarget)
        {
            fruitToEat = Closest(fruits, 15); //fonction qui retourne le fruit le plus proche
            if (fruitToEat != null) seeTarget = true;

            //si la premiere recherche ne marche pas, on se déplace
            if(!blob.hasPath && !seeTarget)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10;
                randomDirection += blob.transform.position;
                blob.SetDestination(randomDirection);
            }

        }
        //Si il a une cible il y va
        if (seeTarget && fruitToEat != null)
        {
            blob.SetDestination(fruitToEat.transform.position);
        }

        //quand il arrive au fruit
        if (seeTarget && fruitToEat != null && blob.hasPath && blob.remainingDistance < 1)
        {
            blob.ResetPath();
            seeTarget = false;
            Eat(fruitToEat.GetComponent<FruitComponent>());
        }

        _state = BlobState.None;
    }






    void SearchPartner()
    {
        GameObject[] blobs = GameObject.FindGameObjectsWithTag("Blob");
        if (!seeTarget)
        {
            target = Closest(blobs, 50);
            if (target != null) seeTarget = true; 
            //si la premiere recherche ne marche pas, on se déplace
            if (!seeTarget)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10;
                randomDirection += blob.transform.position;
                blob.SetDestination(randomDirection);
            }
        }
        //Si il a une cible il y va
        if (seeTarget && target != null)
        {
            //Debug.Log("Set destination");
            blob.SetDestination(target.transform.position);
            
        }

        if(seeTarget && target != null && blob.hasPath && blob.remainingDistance < 1)
        {
            if (target.GetComponent<BlobComponent>().isFertile())
            {
                Debug.Log("Copulation");
                Reproduce(target.GetComponent<BlobComponent>());
            }
            blob.ResetPath();
            seeTarget = false;
            target = null;
        }

        _state = BlobState.None;
    }



    public GameObject Closest(GameObject[] articles, int maxDist)
    {
        GameObject closest = null;
        float minDist = maxDist;
        foreach (GameObject article in articles)
        {
            float dist = Vector3.Distance(blob.transform.position, article.transform.position);
            if (dist < maxDist && dist < minDist)
            {
                closest = article;
                minDist = dist;
            }
        }

        return closest;
    }



    void Eat(FruitComponent fruit)
    {
        energy += fruit.calories;
        Vector2 newPos = Random.insideUnitCircle * 50;
        fruit.transform.position += new Vector3(newPos.x, 0f, newPos.y);
        //pour être sûr que le fruit sorte pas du terrain
        if (fruit.transform.position.x < 0) fruit.transform.position -= new Vector3(fruit.transform.position.x, 0, 0);
        if (fruit.transform.position.z < 0) fruit.transform.position -= new Vector3(0, 0, fruit.transform.position.z);
    }

    public bool isFertile()
    {
        if (energy >= 70) return true;
        else return false;
    }

    void Reproduce(BlobComponent partner)
    {
        Debug.Log("Ils font des bebes");
        energy -= 50;
    }

}
