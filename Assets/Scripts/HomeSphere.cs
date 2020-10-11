﻿using System;
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
        if (delta > 1)
        {
            var blockInteractor = other.GetComponent<BlockInteractor>();
            var isSomeInteractionObject = blockInteractor != null && blockInteractor.IsActivated();
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