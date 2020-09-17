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
    private Vector3 _originalLocalPosition;
    private Vector3 _originalScale;
    private WorldPlane _worldPlane;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _followObject = GetComponent<FollowObject>();
        _originalLocalPosition = transform.localPosition;
        _originalScale = transform.localScale;
        _worldPlane = GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();

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
        transform.localScale = _originalScale * 2f;
    }

    public void Deactivate()
    {
        _followObject.enabled = false;
        _frozen = true;
        transform.localScale = _originalScale;

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
        _audioSource.Stop();
        
        if (volumeOverride > 0)
        {
            _audioSource.PlayOneShot(blockGeneralInteractionSound, volumeOverride);
        }
        else
        {
            _audioSource.PlayOneShot(blockGeneralInteractionSound);
        }
    }

    public void PlayMiscSound(AudioClip clip)
    {
        _audioSource.Stop();

        _audioSource.PlayOneShot(clip);
    }

    public void StopGeneralSound()
    {
        _audioSource.Stop();
    }

    public void ResetPosition()
    {
        transform.localPosition = _originalLocalPosition;
    }

    protected WorldPlane GetWorldPlane()
    {
        return _worldPlane;
    }
}