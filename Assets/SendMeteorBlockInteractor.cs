using UnityEngine;

public class SendMeteorBlockInteractor : BlockInteractor
{
    public Camera mainCamera;
    public GameObject meteorTemplate;
    public AudioClip kaboomSound;
    
    private Meteor _meteor;

    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }
    
    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        var isBlock = other.gameObject.GetComponent<Block>();
        if (!isBlock) return;
        
        if (_meteor == null)
        {
            var meteorGameObject = Instantiate(meteorTemplate);
            var meteor = meteorGameObject.GetComponent<Meteor>();
            meteor.SetTarget(other.gameObject.transform.position);
            meteor.Shoot(mainCamera.transform.position);

            PlayGeneralSound();
            meteor.BeforeDestroy += () => { };
            meteor.Hit += PlayKaboom;

            _meteor = meteor;
        }
    }

    public void PlayKaboom()
    {
        PlayMiscSound(kaboomSound);
    }
}