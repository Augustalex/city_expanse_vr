using UnityEngine;

public class SendMeteorBlockInteractor : BlockInteractor
{
    public Camera mainCamera;
    public GameObject meteorTemplate;
    public AudioClip kaboomSound;

    private Meteor _meteor;

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

    public override void Interact(GameObject other)
    {
        var meteorGameObject = Instantiate(meteorTemplate);
        var meteor = meteorGameObject.GetComponent<Meteor>();
        meteor.SetTarget(other.gameObject.transform.position);
        meteor.Shoot(mainCamera.transform.position);

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