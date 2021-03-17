using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlobPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameField;
    [SerializeField]
    TextMeshProUGUI speedField;
    [SerializeField]
    TextMeshProUGUI sensorsSizeField;
    [SerializeField]
    TextMeshProUGUI energyNeedField;
    [SerializeField]
    TextMeshProUGUI groupField;
    [SerializeField]
    TextMeshProUGUI generosityField;

    public void UpdateInfos(Blob blob)
    {
        nameField.text = blob.firstname;
        speedField.text = "Speed : "+(blob.gene_speed).ToString();
        sensorsSizeField.text = "Sensors size : " + (blob.gene_size).ToString();
        energyNeedField.text = "Energy needs : " + (blob.gene_energyNeeds).ToString();
        UpdateGroupField(blob.group.members);
        generosityField.text = "Generosity : " + (blob.gene_share).ToString();

    }

    public void UpdateGroupField(List<Blob> group)
    {
        groupField.text = "Group : ";
        foreach(Blob b in group)
        {
            groupField.text += (b.firstname + "-");
        }
        groupField.text = groupField.text.Substring(0, groupField.text.Length-2);
    }
}
