using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class CustomButton : MonoBehaviour
{
    [SerializeField]
    bool isSelected = true;
    Button btn;
    Image btnImage;
    Color baseColor;
    Color selectedColor;

    [SerializeField]
    GameObject chart;

    private void Start()
    {
        btn = GetComponent<Button>();
        btnImage = GetComponent<Image>();
        baseColor = btn.colors.normalColor;
        selectedColor = btn.colors.disabledColor;

        Toogle();
    }
    public void Toogle()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            btnImage.color = selectedColor;
        }
        else
        {
            btnImage.color = baseColor;
        }

        chart.SetActive(isSelected);
    }
}
