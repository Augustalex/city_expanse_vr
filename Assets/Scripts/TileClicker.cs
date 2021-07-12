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

        _interactorHolder.GetInteractor().Inspect(hit.collider.gameObject);
    }

    private void StartRayInteraction()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        if (hit.collider.CompareTag("CloudCollisionBox"))
        {
            StartCloudRayInteraction(hit);
        }
        else if (_interactorHolder.AnyInteractorActive())
        {
            if (_interactorHolder.GetInteractor().Interactable(hit.collider.gameObject))
            {
                StartBlockRayInteraction(hit);
            }
        }
    }

    private void StartBlockRayInteraction(RaycastHit hit)
    {
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

    private void StartCloudRayInteraction(RaycastHit hit)
    {
        hit.collider.transform.parent.GetComponent<CloudMover>().CloudMouseDown();
    }

    private void ResetCooldown()
    {
        _coolingDown = false;
    }
}