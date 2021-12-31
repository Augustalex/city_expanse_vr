using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSpawn : MonoBehaviour
{
    public GameObject[] shrineTemplates;

    private static int _shrineCount = 0;

    void Awake()
    {
        SetupHouse(shrineTemplates);
    }

    void Start()
    {
        if (_shrineCount == 0)
        {
            var discoveryManager = DiscoveryManager.Get();
            if (discoveryManager)
            {
                discoveryManager.RegisterNewDiscover(DiscoveryManager.Discoverable.ForrestShrine);
            }
        }

        _shrineCount += 1;
    }

    private void SetupHouse(GameObject[] templates)
    {
        var houseTemplate = templates[Random.Range(0, templates.Length)];
        Instantiate(houseTemplate, transform, false);
    }
}