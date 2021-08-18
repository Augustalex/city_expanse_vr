using System;
using JetBrains.Annotations;
using UnityEngine;

public class SendMeteorBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.SendMeteor;

    [HideInInspector]
    public Camera mainCamera = null;
    
    public GameObject meteorTemplate;
    public AudioClip kaboomSound;

    [CanBeNull] public Transform meteorStartingPosition;
    
    private Meteor _meteor;

    new void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var isBlock = other.gameObject.GetComponent<Block>();
        if (!isBlock) return false;

        return _meteor == null;
    }

    public override bool LockOnLayer()
    {
        return false;
    }

    public void Interact(GameObject other, Action afterMeteorHasHit)
    {
        Interact(other);
        _meteor.BeforeDestroy += afterMeteorHasHit;
    }
    
    public override void Interact(GameObject other)
    {
        var meteorGameObject = Instantiate(meteorTemplate);
        var meteor = meteorGameObject.GetComponent<Meteor>();
        meteor.SetTarget(other.gameObject.transform.position);
        meteor.Shoot(meteorStartingPosition != null ? meteorStartingPosition.position : mainCamera.transform.position);

        PlaySound(BlockSoundLibrary.BlockSound.Meteor);
        meteor.BeforeDestroy += () => { };
        meteor.Hit += PlayKaboom;

        _meteor = meteor;
    }

    public void PlayKaboom()
    {
        PlayMiscSound(kaboomSound);
    }
}