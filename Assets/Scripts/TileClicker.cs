using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(InteractorHolder))]
public class TileClicker : MonoBehaviour
{
    [HideInInspector] public Camera mainCamera = null;

    private InteractorHolder _interactorHolder;
    private bool _coolingDown;
    private float _cooldownTimeLeft;
    private float _lastLayer = 999f;
    private CloudMover _cloudMover;
    private MenuScene _menuScene;
    private DiscoveryScene _discoveryScene;
    private const float CooldownTime = .4f;

    private bool _preventInteractionCooldown;
    private float _preventInteractionCooldownTimeLeft;

    void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
        _cloudMover = FindObjectOfType<CloudMover>();
        mainCamera = Camera.main;

        _menuScene = MenuScene.Get();
        _discoveryScene = DiscoveryScene.Get();
    }

    void Update()
    {
        if (GetPointerDown())
        {
            if (_interactorHolder.AnyInteractorActive())
            {
                HideInteractorGhost();

                if (Input.GetKey(KeyCode.LeftControl)) return;
            }

            if (!_discoveryScene.Visible() && !_menuScene.IsShowing())
            {
                StartRayInteraction();
            }
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
        
        if (_preventInteractionCooldown)
        {
            _preventInteractionCooldownTimeLeft -= Time.deltaTime;
            if (_preventInteractionCooldownTimeLeft < 0)
            {
                _preventInteractionCooldown = false;
            }
        }
    }

    private void HideInteractorGhost()
    {
        _interactorHolder.GetInteractor().HideGhost();
    }

    private Vector2 GetPointerPosition()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2) Input.mousePosition;
    }

    private bool GetPointerDown()
    {
        return Input.GetMouseButton(0) || (Input.touchCount == 1) || Input.GetMouseButton(1);
    }

    public bool IsRightClick()
    {
        return Input.GetMouseButton(1);
    }

    private void StartRayInspection()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(GetPointerPosition());
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        var blockRigidbody = hit.collider.attachedRigidbody;
        if (blockRigidbody)
        {
            _interactorHolder.GetInteractor().Inspect(blockRigidbody.gameObject);
        }
    }

    private void StartRayInteraction()
    {
        if (_preventInteractionCooldown) return;
        
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(GetPointerPosition());
        if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

        if (hit.collider.CompareTag("CloudCollisionBox"))
        {
            StartCloudRayInteraction();
        }
        else if (hit.collider.CompareTag("LakeSpawnHighlight"))
        {
            var lakeSpawn = hit.collider.gameObject.GetComponent<LakeSpawnHighlight>().GetLakeSpawn();
            if (IsRightClick())
            {
                lakeSpawn.DestroyLakeSpawn();
            }
            else
            {
                LakeSpawner.Get().ActivateLakeSpawn(lakeSpawn);
            }
            
            _preventInteractionCooldown = true;
            _preventInteractionCooldownTimeLeft = .25f;
        }
        else if (!_cloudMover.IsMovingWithMouse())
        {
            if (_interactorHolder.AnyInteractorActive())
            {
                var blockRigidbody = hit.collider.attachedRigidbody;
                if (blockRigidbody)
                {
                    var target = blockRigidbody.gameObject;
                    if (target.CompareTag("GreensSpawn"))
                    {
                        Debug.Log("GREENSSPAWN!");
                        target = target.GetComponent<GreensSpawn>().GetBlockRelative().gameObject;
                    }
            
                    if (_interactorHolder.GetInteractor().Interactable(target))
                    {
                        StartBlockRayInteraction(target);
                    }
                }
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

            
            _preventInteractionCooldown = true;
            _preventInteractionCooldownTimeLeft = .075f;
            
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