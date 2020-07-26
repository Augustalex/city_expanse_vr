﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteractor : MonoBehaviour
{
    public bool isStartingInteractor = false;

    public AudioClip blockGeneralInteractionSound;

    private AudioSource _audioSource;
    private FollowObject _followObject;
    private bool _frozen = false;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _followObject = GetComponent<FollowObject>();

        if (isStartingInteractor)
        {
            GetComponentInParent<BlockInteractionPalette>().Select(this);
            Activate();
        }
    }

    public void Activate()
    {
        _followObject.enabled = true;
        _frozen = false;
    }

    public void Deactivate()
    {
        _followObject.enabled = false;
        _frozen = true;
        StartCoroutine(UnfreezeSoon());

        IEnumerator UnfreezeSoon()
        {
            yield return new WaitForSeconds(1);
            _frozen = false;
        }
    }

    public bool IsActivated()
    {
        return _followObject.enabled;
    }
    
    public bool IsInteractable()
    {
        return !_frozen;
    }

    public void PlayGeneralSound()
    {
        _audioSource.PlayOneShot(blockGeneralInteractionSound);
    }
}