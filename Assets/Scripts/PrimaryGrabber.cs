using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrimaryGrabber : MonoBehaviour
{
    private Cloud _nearbyCloud;
    private float _enteredSecondaryGrabberLast = 0;
    private bool _touchingSecondaryGrabber;
    private bool _grabbedCloud;
    private Rigidbody _rigidbody;
    private double _latestMagnitude;
    private double _latestSampleTime;

    private const float GrabberMinThreshold = .2f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (_grabbedCloud && !_touchingSecondaryGrabber && Time.fixedTime - _enteredSecondaryGrabberLast > GrabberMinThreshold)
        {
            ReleaseCloud();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var isSecondaryGrabber = other.gameObject.GetComponent<SecondaryGrabber>();
        if (isSecondaryGrabber)
        {
            _enteredSecondaryGrabberLast = Time.fixedTime;
            _touchingSecondaryGrabber = true;

            var cloud = GetNearbyCloudHook();
            if (cloud != null)
            {
                _nearbyCloud = cloud;
                GrabCloud();
            }
        }
    }

    private void GrabCloud()
    {
        if (_nearbyCloud != null)
        {
            _grabbedCloud = true;
            _nearbyCloud.Hook(transform);
        }
    }

    private Cloud GetNearbyCloudHook()
    {
        var hits = Physics.OverlapSphere(transform.position, .2f);
        return hits
            .FirstOrDefault(hit => hit.gameObject.GetComponentInParent<Cloud>() != null)
            ?.gameObject.GetComponentInParent<Cloud>();
    }

    private void OnTriggerExit(Collider other)
    {
        var isSecondaryGrabber = other.gameObject.GetComponent<SecondaryGrabber>();
        if (isSecondaryGrabber)
        {
            _touchingSecondaryGrabber = false;

            var timeSinceEntered = Time.fixedTime - _enteredSecondaryGrabberLast;
            if (timeSinceEntered > GrabberMinThreshold)
            {
                ReleaseCloud();
            }
        }
    }

    private void ReleaseCloud()
    {
        if (_nearbyCloud != null)
        {
            _grabbedCloud = false;
            _nearbyCloud.UnHook(transform);
        }
    }
}