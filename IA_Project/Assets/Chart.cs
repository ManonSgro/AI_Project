using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chart : MonoBehaviour
{
    public List<float> points = new List<float>();

    [SerializeField]
    GameObject point;
    [SerializeField]
    GameObject text;
    [SerializeField]
    int maxPoints = 30;

    public void UpdatePoints(bool allPoints)
    {
        // Remove current points
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        if (points.Count > 0)
        {
            float max = points.Max() != 0 ? points.Max() : 1f;
            float gradientY = GetComponent<RectTransform>().rect.height / max;
            if(name == "ChartPopulation")
            {
                /*
                for (int n = 0; n <= max; n += 10)
                {
                    var posY = n * gradientY + text.GetComponent<RectTransform>().rect.height;
                    GameObject tmpText = Instantiate(text, new Vector3(transform.position.x - GetComponent<RectTransform>().rect.width / 2, posY, 0f), Quaternion.identity, transform);
                    tmpText.GetComponent<TextMeshProUGUI>().text = n.ToString();
                }
                */
            }

            int start = 0;
            float gradientX = GetComponent<RectTransform>().rect.width / points.Count;
            if (allPoints && points.Count > maxPoints)
            {
                start = points.Count - maxPoints;
                gradientX = GetComponent<RectTransform>().rect.width / maxPoints;
            }

            if (gradientX > 20)
            {
                gradientX = 20;
            }

            for (int i = start; i < points.Count; ++i)
            {
                var posY = points[i] * gradientY;
                var posX = (i - start + 1) * gradientX;
                var tmpBar = Instantiate(point, transform.position, Quaternion.identity, transform);
                tmpBar.GetComponent<Point>().value = points[i];
                var posYfinal = posY - GetComponent<RectTransform>().rect.height / 2 - tmpBar.GetComponent<RectTransform>().rect.height;
                //Debug.Log(gradientY);
                tmpBar.transform.localPosition = new Vector3(posX - GetComponent<RectTransform>().rect.width / 2 - tmpBar.GetComponent<RectTransform>().rect.width / 2, posYfinal, 0f);
            }
        }
    }
}
