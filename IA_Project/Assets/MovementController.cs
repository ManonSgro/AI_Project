using UnityEngine;

public class MovementController : MonoBehaviour
{
    void Update()
    {
        float posX = Input.GetAxis("Vertical") * 20f * Time.deltaTime;
        float posZ = Input.GetAxis("Horizontal") * 20f * Time.deltaTime;
        transform.Translate(posZ, 0f, posX);
    }
}
