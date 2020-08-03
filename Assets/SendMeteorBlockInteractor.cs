using UnityEngine;

public class SendMeteorBlockInteractor : BlockInteractor
{
    public GameObject meteorTemplate;
    private Meteor _meteor;

    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        if (_meteor == null)
        {
            var meteorGameObject = Instantiate(meteorTemplate);
            var meteor = meteorGameObject.GetComponent<Meteor>();
            meteor.SetTarget(other.gameObject.transform.position);
            meteor.Shoot(transform.position);

            _meteor = meteor;
        }
        else
        {
            _meteor.SetTarget(other.gameObject.transform.position);
        }
    }
}