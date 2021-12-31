using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    
    private static int _fortCount = 0;

    void Awake()
    {
        SetupHouse(tinyHouseTemplates);
    }

    void Start()
    {
        if (_fortCount == 0)
        {
            var discoveryManager = DiscoveryManager.Get();
            if (discoveryManager)
            {
                DiscoveryManager.Get().RegisterNewDiscover(DiscoveryManager.Discoverable.CliffHouse);
            }
        }

        _fortCount += 1;
    }
    
    private void SetupHouse(GameObject[] templates)
    {
        var houseTemplate = templates[Random.Range(0, templates.Length)];
        Instantiate(houseTemplate, transform, false);
    }
}
