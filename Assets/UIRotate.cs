using UnityEngine;


public class UIRotate : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, 45f * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.back, 20f * Time.deltaTime);
        // transform.RotateAround(transform.position, Vector3.forward, 24f * Time.deltaTime);
    }
}
