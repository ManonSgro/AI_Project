using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ContactZone : MonoBehaviour
{
    [SerializeField]
    Blob blob;
    [SerializeField]
    GameObject babyBlob;

    GameObject gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (blob.state == BlobState.Hungry && other.CompareTag("Edible"))
        {
            if (blob.child)
            {
                float scale = 0.1f / 20 * other.GetComponent<Fruit>().calories;
                blob.transform.localScale = blob.transform.localScale + new Vector3(scale, scale, scale);
                if (blob.transform.localScale.x >= 1)
                {
                    blob.child = false;
                }
            }
            blob.energy += other.GetComponent<Fruit>().calories;
            Destroy(other.gameObject);
        }
        if (blob.state == BlobState.Fertile && other.CompareTag("Blob") && other.GetComponent<Blob>().state == BlobState.Fertile)
        {
            gameManager.GetComponent<Abilities>().AddParents(blob, other.GetComponent<Blob>());
            blob.StartCoroutine("MakingBaby");
        }
        if (blob.state == BlobState.Fertile && other.CompareTag("Blob") && other.GetComponent<Blob>().state == BlobState.Hungry)
        {
            float random = Random.Range(0f, 1f);
            if (random <= blob.gene_share)
            {
                float energyToShare = (blob.energy - 50) / 2;
                if(energyToShare >= 1)
                {
                    other.GetComponent<Blob>().energy += energyToShare;
                    blob.energy -= energyToShare;
                    if (other.GetComponent<Blob>().child)
                    {
                        float scale = 0.1f / 20 * energyToShare;
                        other.transform.localScale += new Vector3(scale, scale, scale);
                        if (other.transform.localScale.x >= 1)
                        {
                            other.GetComponent<Blob>().child = false;
                        }
                    }
                }
            }
        }
        if (other.CompareTag("Blob") && !blob.group.members.Contains(other.GetComponent<Blob>()) && blob.group.members.Capacity - blob.group.members.Count >= other.GetComponent<Blob>().group.members.Count && other.GetComponent<Blob>().group.members.Capacity - other.GetComponent<Blob>().group.members.Count >= blob.group.members.Count)
        {
            var newGroup = blob.group.MergeGroups(other.GetComponent<Blob>().group);
            blob.group = newGroup;
            other.GetComponent<Blob>().group = newGroup;
            //other.GetComponent<Blob>().group.Add(blob);
        }       
    }
}
