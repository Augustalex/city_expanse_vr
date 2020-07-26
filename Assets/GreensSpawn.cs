using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreensSpawn : MonoBehaviour
{
    public GameObject[] greensTemplates;

    void Awake()
    {
        var greensTemplate = greensTemplates[Random.Range(0, greensTemplates.Length)];
        var greens = Instantiate(greensTemplate);
        greens.transform.SetParent(transform, false);
        greens.transform.Rotate(new Vector3(0, Random.value * 360, 0));
    }
}
