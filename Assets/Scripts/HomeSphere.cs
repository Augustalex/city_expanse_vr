using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSphere : MonoBehaviour
{
    public AudioClip recenterSound;
    private float _enterTime;

    private void OnTriggerEnter(Collider other)
    {
        _enterTime = Time.fixedTime;
    }

    private void OnTriggerExit(Collider other)
    {
        var delta = Time.fixedTime - _enterTime;
        if (delta > .2f)
        {
            var blockInteractor = other.GetComponent<BlockInteractor>();
            var isSomeInteractionObject = blockInteractor != null && blockInteractor.IsActivated();
            if (isSomeInteractionObject)
            {
                var realCenterPoint = WorldPlane.Get().GetRealCenterPoint();
                
                var parentTransform = Camera.main.gameObject.transform.parent.gameObject.transform;
                
                parentTransform.position = new Vector3(
                    0,
                    parentTransform.position.y,
                    0
                    );
                var diff = Camera.main.transform.position - realCenterPoint;
                diff.y = 0;
                parentTransform.position -= diff;

                var currentPosition = parentTransform.position;
                parentTransform.position = new Vector3(currentPosition.x, .4f, currentPosition.z); 

                GetComponent<AudioSource>().PlayOneShot(recenterSound);
            }
        }
    }
}