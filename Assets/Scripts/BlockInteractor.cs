using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FollowMainHandInteractor))]
[RequireComponent(typeof(AudioSource))]
public abstract class BlockInteractor : MonoBehaviour
{
    public bool isStartingInteractor = false;

    private AudioSource _audioSource;
    private FollowMainHandInteractor _followObject;
    private bool _frozen;
    private Vector3 _originalLocalPosition;
    private Vector3 _originalScale;
    private WorldPlane _worldPlane;
    private bool _started;

    private List<Block> _highlighted = new List<Block>();
    private string _identifier = "uninstantiated";

    private FeatureToggles _featureToggles;

    protected bool showInteractionGhost = false;

    private GameObject _ghost;
    private GameObject _nonInteractableGhost;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _followObject = GetComponent<FollowMainHandInteractor>();
        _identifier = Math.Round(Random.value * 9999999).ToString();
    }

    protected void Start()
    {
        _originalLocalPosition = transform.localPosition;

        _originalScale = transform.localScale * 1.5f;
        transform.localScale = _originalScale;

        _featureToggles = FeatureToggles.Get();
        _worldPlane = WorldPlane.Get();

        if (isStartingInteractor)
        {
            GetComponentInParent<BlockInteractionPalette>().Select(this);
            Activate();
        }
        
        
        _ghost = Instantiate(BlockFactory.Get().interactableGhostTemplate);
        _ghost.SetActive(false);

        _nonInteractableGhost = Instantiate(BlockFactory.Get().nonInteractableGhostTemplate);
        _nonInteractableGhost.SetActive(false);

        _started = true;
    }

    protected void Update()
    {
        if (_featureToggles.interactionOverlay && _highlighted.Count > 0)
        {
            var newList = new List<Block>();
            foreach (var b in _highlighted)
            {
                if (b == null) continue;

                if (b.ShouldRemoveHighlight(_identifier))
                {
                    b.RemoveHighlight();
                }
                else
                {
                    newList.Add(b);
                }
            }

            _highlighted = newList;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Interactable(other.gameObject))
        {
            ResurrectNearbyBlocks(other.GetComponent<Block>().GetGridPosition());
            Interact(other.gameObject);
        }
    }

    public void ResurrectNearbyBlocks(Vector3 gridPosition)
    {
        var nearbyWaterBlocks = _worldPlane.GetNearbyBlocksWithinRange(gridPosition, 4f)
            .Where(block => block.IsOutsideWater() || block.IsWater());
        foreach (var waterBlock in nearbyWaterBlocks)
        {
            waterBlock.GetComponent<FloodingWater>().Resurrect();
        }
    }

    public abstract bool LockOnLayer();

    public abstract void Interact(GameObject other);

    public abstract bool Interactable(GameObject other);

    public void Inspect(GameObject other)
    {
        if (!_started) return;

        if (_featureToggles.interactionOverlay)
        {
            InspectWithOverlay(other);
        }
        
        if(_featureToggles.interactionGhost && showInteractionGhost)
        {
            InspectWithGhost(other);
        }
    }

    private void InspectWithOverlay(GameObject other)
    {
        _highlighted.ForEach(h =>
        {
            if (h != null) h.RemoveHighlight();
        });
        _highlighted.Clear();

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (!blockComponent) return;

        var nearbyBlocks = GetWorldPlane().GetNearbyBlocksWithinRange(blockComponent.GetGridPosition(), 5f);
        foreach (var nearbyBlock in nearbyBlocks)
        {
            if (Interactable(nearbyBlock.gameObject))
            {
                nearbyBlock.Highlight(_identifier, Block.HighlightType.Interactable);
                _highlighted.Add(nearbyBlock);
            }
            else
            {
                nearbyBlock.Highlight(_identifier, Block.HighlightType.NonInteractable);
                _highlighted.Add(nearbyBlock);
            }
        }
    }
    
    private void InspectWithGhost(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();
        if (!blockComponent) return;

        if (Interactable(other))
        {
            if (_nonInteractableGhost.activeSelf) _nonInteractableGhost.SetActive(false);
            if (!_ghost.activeSelf) _ghost.SetActive(true);
            _ghost.transform.position = other.transform.position + Vector3.up * .15f;
        }
        else
        {
            if (_ghost.activeSelf) _ghost.SetActive(false);
            if (!_nonInteractableGhost.activeSelf) _nonInteractableGhost.SetActive(true);
            _nonInteractableGhost.transform.position = other.transform.position + Vector3.up * .15f;
        }
    }

    public void Activate()
    {
        _followObject.enabled = true;
        _frozen = false;
        transform.localScale = _originalScale * 1.5f;
    }

    public void DeactivateFromPalette()
    {
        Deactivate();

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

    public void Deactivate()
    {
        if (_started && _featureToggles.interactionGhost && showInteractionGhost)
        {
            _nonInteractableGhost.SetActive(false);
            _ghost.SetActive(false);
        }
        
        enabled = false;
    }

    public bool IsActivated()
    {
        return enabled || _followObject.enabled;
    }

    public bool IsInteractable()
    {
        return !_frozen;
    }

    public void PlayGeneralSound()
    {
        PlaySound(BlockSoundLibrary.BlockSound.Basic);
    }

    public void PlaySound(BlockSoundLibrary.BlockSound blockSound, Vector3? position = null)
    {
        _audioSource.Stop();

        var sound = BlockSoundLibrary.Get().GetSound(blockSound);

        if (position != null)
        {
            AudioSource.PlayClipAtPoint(sound, position.Value, .02f * GameManager.MasterVolume);
        }
        else
        {
            _audioSource.PlayOneShot(sound, .02f * GameManager.MasterVolume);
        }
    }

    public void PlayMiscSound(AudioClip clip)
    {
        _audioSource.Stop();

        _audioSource.PlayOneShot(clip, .02f * GameManager.MasterVolume);
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

    public void HideGhost()
    {
      if(_nonInteractableGhost.activeSelf) _nonInteractableGhost.SetActive(false);
      if(_ghost.activeSelf) _ghost.SetActive(false);
    }
}