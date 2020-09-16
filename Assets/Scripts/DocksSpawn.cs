using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocksSpawn : MonoBehaviour
{
    public GameObject[] docksTemplates;

    void Start()
    {
        var docksTemplate = docksTemplates[Random.Range(0, docksTemplates.Length)];
        var docks = Instantiate(docksTemplate);
        docks.transform.SetParent(transform, false);
    }
}
