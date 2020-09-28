using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowMainHandInteractor))]
[RequireComponent(typeof(AudioSource))]
public abstract class BlockInteractor : MonoBehaviour
{
    public bool isStartingInteractor = false;

    public float volumeOverride = 0;

    private AudioSource _audioSource;
    private FollowMainHandInteractor _followObject;
    private bool _frozen = false;
    private Vector3 _originalLocalPosition;
    private Vector3 _originalScale;
    private WorldPlane _worldPlane;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _followObject = GetComponent<FollowMainHandInteractor>();
        _originalLocalPosition = transform.localPosition;

        _originalScale = transform.localScale * 1.5f;
        transform.localScale = _originalScale;
        
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
        transform.localScale = _originalScale * 1.5f;
    }

    public void Deactivate()
    {
        _followObject.enabled = false;
        _frozen = true;
        transform.localScale = _originalScale;

        StartCoroutine(UnfreezeSoon());

        IEnumerator UnfreezeSoon()
        {
            yield return new WaitForSeconds(.5f);
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
        PlaySound(BlockSoundLibrary.BlockSound.Basic);
    }

    public void PlaySound(BlockSoundLibrary.BlockSound blockSound)
    {
        _audioSource.Stop();
        
        var sound = BlockSoundLibrary.Get().GetSound(blockSound);
        if (volumeOverride > 0)
        {
            _audioSource.PlayOneShot(sound, volumeOverride);
        }
        else
        {
            _audioSource.PlayOneShot(sound);
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