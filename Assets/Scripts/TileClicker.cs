using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractorHolder))]
public class TileClicker : MonoBehaviour
{
    public Camera mainCamera;

    private InteractorHolder _interactorHolder;
    private bool _coolingDown;
    private float _cooldownTimeLeft;
    private float _lastLayer = 999f;
    private const float CooldownTime = 2f;

    void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.LeftControl)) return;

            StartRayInteraction();
        }
        else
        {
            ResetCooldown();
        }

        if (_coolingDown)
        {
            _cooldownTimeLeft -= Time.deltaTime;
            if (_cooldownTimeLeft < 0)
            {
                ResetCooldown();
            }
        }
    }

    private void StartRayInteraction()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        if (!_interactorHolder.GetInteractor().Interactable(hit.collider.gameObject)) return;

        var block = hit.collider.gameObject.GetComponent<Block>();
        if (!block) return;

        var layer = block.GetGridPosition().y;

        var sameLayerAsBefore = Math.Abs(_lastLayer - layer) < .5f;
        if (!_interactorHolder.GetInteractor().LockOnLayer() || !_coolingDown || sameLayerAsBefore)
        {
            var colliderGameObject = hit.collider.gameObject;
            _interactorHolder.GetInteractor()
                .ResurrectNearbyBlocks(colliderGameObject.GetComponent<Block>().GetGridPosition());
            _interactorHolder.GetInteractor().Interact(colliderGameObject);

            _lastLayer = layer;
            _coolingDown = true;
            _cooldownTimeLeft = CooldownTime;
        }
    }

    private void ResetCooldown()
    {
        _coolingDown = false;
    }
}