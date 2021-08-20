using System;
using System.Collections.Generic;
using UnityEngine;

public class DiscoveryManager : MonoBehaviour
{
    public enum Discoverable
    {
        House,
        BigHouse,
        Docks,
        Farm,
        Silo,
        CliffHouse,
        ForrestShrine
    }

    private readonly Dictionary<Discoverable, bool> _discoveries = new Dictionary<Discoverable, bool>();

    public event Action<Discoverable> NewDiscover;
    
    private static DiscoveryManager _instance;

    void Awake()
    {
        _instance = this;
    }

    public static DiscoveryManager Get()
    {
        return _instance;
    }

    public void RegisterNewDiscover(Discoverable discoverable)
    {
        _discoveries[discoverable] = true;

        OnNewDiscover(discoverable);
    }

    public bool IsDiscovered(Discoverable discoverable)
    {
        if (!_discoveries.ContainsKey(discoverable))
        {
            _discoveries[discoverable] = false;
        }
        
        return _discoveries[discoverable];
    }

    protected virtual void OnNewDiscover(Discoverable obj)
    {
        NewDiscover?.Invoke(obj);
    }
}