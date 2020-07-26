using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSphere : MonoBehaviour
{
    public AudioClip recenterSound;
    public GameObject camera;
    private float _enterTime;

    private void OnTriggerEnter(Collider other)
    {
        _enterTime = Time.fixedTime;
    }

    private void OnTriggerExit(Collider other)
    {
        var delta = Time.fixedTime - _enterTime;
        if (delta > 2)
        {
            var followObjectComponent = other.GetComponent<FollowObject>();
            var isSomeInteractionObject = followObjectComponent != null && followObjectComponent.enabled;
            if (isSomeInteractionObject)
            {
                camera.transform.position = new Vector3(0, camera.transform.position.y, 0);
                var diff = Camera.main.transform.position - camera.transform.position;
                diff.y = 0;
                camera.transform.position -= diff;
                GetComponent<AudioSource>().PlayOneShot(recenterSound);
            }
        }
    }
}