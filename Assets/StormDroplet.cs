using System.Collections;
using blockInteractions;
using UnityEngine;

public class StormDroplet : MonoBehaviour
{
    private void Update()
    {
        StartCoroutine(DestroySoon());
    }

    private IEnumerator DestroySoon()
    {
        yield return new WaitForSeconds(5);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Random.value < .001f)
        {
            var block = other.GetComponent<Block>();
            RaiseWater.Get().Use(block);
            Destroy(gameObject);
        }
    }
}