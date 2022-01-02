using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireController : MonoBehaviour
{
    public AudioClip upgradeSound;

    private ParticleSystem _particleSystem;

    private float _originalStartLifetime;
    private Animator _animator;
    private static readonly int Highlight = Animator.StringToHash("Highlight");
    private BonfireStateManager _bonfireStateManager;

    void Awake()
    {
        _bonfireStateManager = BonfireStateManager.Get();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _animator = GetComponent<Animator>();
        _originalStartLifetime = _particleSystem.main.startLifetime.constant;
    }

    private void Start()
    {
        ConstructionMediator.Get().OnBuildingCreated += BuildingCreated;
    }

    private void BuildingCreated(BuildingInfo info)
    {
        _bonfireStateManager.SetDevotees(_bonfireStateManager.GetDevoteeCount() + info.devotees);

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(1.15f);
            RunHighlightAnimation();
        }
    }

    private void RunHighlightAnimation()
    {
        _animator.SetTrigger(Highlight);
    }

    public void UpdateBonfireSize()
    {
        var main = _particleSystem.main;
        main.startLifetime = _originalStartLifetime + 1.5f * _bonfireStateManager.GetDevoteeCount();
    }

    public void PlayUpgradeSound()
    {
        GetAudioSource().PlayOneShot(upgradeSound, .02f * GameManager.MasterVolume);
    }

    private AudioSource GetAudioSource()
    {
        return GetComponentInParent<AudioSource>();
    }
}