using System;
using UnityEngine;

public class FireworksEffect : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;
    private bool _activated;

    public event Action ActivatedHit;

    void Start()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Activate()
    {
        _activated = true;
    }

    public void SetHitBoxSize(float size)
    {
        GetComponent<BoxCollider>().size = new Vector3(size, size, size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activated)
        {
            foreach (var system in _particleSystems)
            {
                system.Play();
            }
            
            OnActivatedHit();
            _activated = false;
        }
    }

    protected virtual void OnActivatedHit()
    {
        ActivatedHit?.Invoke();
    }
}