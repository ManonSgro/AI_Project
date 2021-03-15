using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Point : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float value = 0f;
    [SerializeField]
    TextMeshProUGUI textObj;

    private void Start()
    {
        textObj.gameObject.SetActive(false);
        textObj.text = value.ToString();
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        textObj.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        textObj.gameObject.SetActive(false);
    }
}
