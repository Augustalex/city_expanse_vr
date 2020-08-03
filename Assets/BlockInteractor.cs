using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowObject))]
[RequireComponent(typeof(AudioSource))]
public abstract class BlockInteractor : MonoBehaviour
{
    public bool isStartingInteractor = false;

    public AudioClip blockGeneralInteractionSound;
    public float volumeOverride = 0;

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

    public abstract void Interact(GameObject other);

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
        if (volumeOverride > 0)
        {
            _audioSource.PlayOneShot(blockGeneralInteractionSound, volumeOverride);
        }
        else
        {
            _audioSource.PlayOneShot(blockGeneralInteractionSound);
        }
    }
}