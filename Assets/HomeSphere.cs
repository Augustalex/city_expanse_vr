using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSphere : MonoBehaviour
{
    public GameObject camera;

    private void OnTriggerEnter(Collider other)
    {
        var followObjectComponent = other.GetComponent<FollowObject>();
        var isSomeInteractionObject = followObjectComponent != null && followObjectComponent.enabled;
        if (isSomeInteractionObject)
        {
            camera.transform.position = new Vector3(0, camera.transform.position.y, 0);
            var diff = Camera.main.transform.position - camera.transform.position;
            diff.y = 0;
            camera.transform.position -= diff;
        }
    }
}
