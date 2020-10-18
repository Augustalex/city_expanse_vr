using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSphere : MonoBehaviour
{
    public AudioClip recenterSound;
    private GameObject _camera;
    private float _enterTime;

    private void Awake()
    {
        _camera = CameraRigContainer.Get().cameraRig;
    }

    private void OnTriggerEnter(Collider other)
    {
        _enterTime = Time.fixedTime;
    }

    private void OnTriggerExit(Collider other)
    {
        var delta = Time.fixedTime - _enterTime;
        if (delta > .1f)
        {
            var blockInteractor = other.GetComponent<BlockInteractor>();
            var isSomeInteractionObject = blockInteractor != null && blockInteractor.IsActivated();
            if (isSomeInteractionObject)
            {
                _camera.transform.position = new Vector3(0, _camera.transform.position.y, 0);
                var diff = Camera.main.transform.position - _camera.transform.position;
                diff.y = 0;
                _camera.transform.position -= diff;

                var currentPosition = _camera.transform.position;
                _camera.transform.position = new Vector3(currentPosition.x, .6f, currentPosition.z);

                GetComponent<AudioSource>().PlayOneShot(recenterSound);
            }
        }
    }
}