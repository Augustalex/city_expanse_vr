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

    public Queue<Discoverable> unpublished = new Queue<Discoverable>();
    private MenuScene _menuScene;
    private float _lastPublished;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _menuScene = MenuScene.Get();
    }

    void Update()
    {
        if (!_menuScene.IsShowing())
        {
            if (unpublished.Count > 0)
            {
                var timeSinceLastShown = Time.time - _lastPublished;
                if (timeSinceLastShown > 2)
                {
                    var next = unpublished.Dequeue();
                    OnNewDiscover(next);
                }
            }
        }
    }

    public static DiscoveryManager Get()
    {
        return _instance;
    }

    public void RegisterNewDiscover(Discoverable discoverable)
    {
        _discoveries[discoverable] = true;

        unpublished.Enqueue(discoverable);
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
        _lastPublished = Time.time;
        NewDiscover?.Invoke(obj);
    }
}