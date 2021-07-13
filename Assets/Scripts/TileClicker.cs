﻿using System;
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
    private CloudMover _cloudMover;
    private const float CooldownTime = 2f;

    void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
        _cloudMover = FindObjectOfType<CloudMover>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (_interactorHolder.AnyInteractorActive())
            {
                HideInteractorGhost();

                if (Input.GetKey(KeyCode.LeftControl)) return;
            }

            StartRayInteraction();
        }
        else
        {
            if (_interactorHolder.AnyInteractorActive())
            {
                StartRayInspection();
            }

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

    private void HideInteractorGhost()
    {
        _interactorHolder.GetInteractor().HideGhost();
    }

    private void StartRayInspection()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        var blockRigidbody = hit.collider.attachedRigidbody;
        if (blockRigidbody)
        {
            _interactorHolder.GetInteractor().Inspect(blockRigidbody.gameObject);
        }
    }

    private void StartRayInteraction()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        if (hit.collider.CompareTag("CloudCollisionBox"))
        {
            StartCloudRayInteraction();
        }
        else if (_interactorHolder.AnyInteractorActive() && !_cloudMover.IsMovingWithMouse())
        {
            var blockRigidbody = hit.collider.attachedRigidbody;
            if (blockRigidbody && _interactorHolder.GetInteractor().Interactable(blockRigidbody.gameObject))
            {
                StartBlockRayInteraction(blockRigidbody.gameObject);
            }
        }
    }

    private void StartBlockRayInteraction(GameObject blockGameObject)
    {
        var block = blockGameObject.GetComponent<Block>();
        if (!block) return;

        var layer = block.GetGridPosition().y;

        var sameLayerAsBefore = Math.Abs(_lastLayer - layer) < .5f;
        if (!_interactorHolder.GetInteractor().LockOnLayer() || !_coolingDown || sameLayerAsBefore)
        {
            _interactorHolder.GetInteractor()
                .ResurrectNearbyBlocks(blockGameObject.GetComponent<Block>().GetGridPosition());
            _interactorHolder.GetInteractor().Interact(blockGameObject);

            _lastLayer = layer;
            _coolingDown = true;
            _cooldownTimeLeft = CooldownTime;
        }
    }

    private void StartCloudRayInteraction()
    {
        _cloudMover.CloudMouseDown();
    }

    private void ResetCooldown()
    {
        _coolingDown = false;
    }
}