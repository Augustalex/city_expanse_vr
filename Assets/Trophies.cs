using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophies : MonoBehaviour
{
    private TrophyMesh[] _trophyMeshes;
    private DiscoveryManager _discoveryManager;

    private void Awake()
    {
        _trophyMeshes = GetComponentsInChildren<TrophyMesh>();
    }
    
    public void Refresh()
    {
        if(!_discoveryManager) _discoveryManager = DiscoveryManager.Get();
        
        foreach (var trophyMesh in _trophyMeshes)
        {
            var discoverableNameToEnum = DiscoverableNameToEnum(trophyMesh.trophyName);
            if (_discoveryManager.IsDiscovered(discoverableNameToEnum))
            {
                trophyMesh.Unveil();
            }
            
            trophyMesh.Refresh();
        }
    }

    public DiscoveryManager.Discoverable DiscoverableNameToEnum(string trophyName)
    {
        switch (trophyName)
        {
            case "House":
                return DiscoveryManager.Discoverable.House;
            case "Big house":
                return DiscoveryManager.Discoverable.BigHouse;
            case "Farm":
                return DiscoveryManager.Discoverable.Farm;
            case "Silo":
                return DiscoveryManager.Discoverable.Silo;
            case "Docks":
                return DiscoveryManager.Discoverable.Docks;
            case "Cliff house":
                return DiscoveryManager.Discoverable.CliffHouse;
            case "Forrest shrine":
                return DiscoveryManager.Discoverable.ForrestShrine;
            default:
                throw new Exception("No trophy mapped to name: " + trophyName);
        }
    }
}
