using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrimaryGrabber : MonoBehaviour
{
    private Cloud _nearbyCloud;

    private void OnTriggerEnter(Collider other)
    {
        var isSecondaryGrabber = other.gameObject.GetComponent<SecondaryGrabber>();
        if (isSecondaryGrabber)
        {
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
        _nearbyCloud.Hook(transform);
    }

    private Cloud GetNearbyCloudHook()
    {
        var hits = Physics.OverlapSphere(transform.position,  .2f);
        return hits
            .FirstOrDefault(hit => hit.gameObject.GetComponent<Cloud>() != null)
            ?.gameObject .GetComponent<Cloud>();
    }

    private void OnTriggerExit(Collider other)
    {
        var isSecondaryGrabber = other.gameObject.GetComponent<SecondaryGrabber>();
        if (isSecondaryGrabber)
        {
            ReleaseCloud();
        }
    }

    private void ReleaseCloud()
    {
        _nearbyCloud.UnHook(transform);
    }
}
