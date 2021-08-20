﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    
    private FireworksEffect _fireworksEffect;
    
    private static int _fortCount = 0;

    void Awake()
    {
        _fireworksEffect = GetComponentInChildren<FireworksEffect>();
        
        SetupHouse(tinyHouseTemplates);
        _fireworksEffect.SetHitBoxSize(5);
        
        StartCoroutine(ActivateFireworksSoon());
        LaunchFromAbove();

        IEnumerator ActivateFireworksSoon()
        {
            yield return new WaitForSeconds(.05f);
            _fireworksEffect.Activate();
        }
    }

    void Start()
    {
        if (_fortCount == 0)
        {
            DiscoveryManager.Get().RegisterNewDiscover(DiscoveryManager.Discoverable.CliffHouse);
        }

        _fortCount += 1;
    }
    
    private void SetupHouse(GameObject[] templates)
    {
        var houseTemplate = templates[Random.Range(0, templates.Length)];
        Instantiate(houseTemplate, transform, false);
    }
    
    private void LaunchFromAbove()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
    }
}
