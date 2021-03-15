using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    public bool isSelected = true;
    Button btn;
    Image btnImage;
    Color baseColor;
    Color selectedColor;

    private void Start()
    {
        btn = GetComponent<Button>();
        btnImage = GetComponent<Image>();
        baseColor = btn.colors.normalColor;
        selectedColor = btn.colors.disabledColor;

        ChangeAppearance();
    }

    public void ToogleButton()
    {
        isSelected = !isSelected;
        ChangeAppearance();
    }

    void ChangeAppearance()
    {
        if (isSelected)
        {
            btnImage.color = selectedColor;
        }
        else
        {
            btnImage.color = baseColor;
        }
    }
}
