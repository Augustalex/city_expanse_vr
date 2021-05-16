using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainHandInteractor : MonoBehaviour
{
    private Transform _followPoint;
    private bool _doNotFollow;

    private void Start()
    {
        var handInteractorFollowPoint = HandInteractorFollowPoint.Get();
        if (handInteractorFollowPoint)
        {
            _followPoint = handInteractorFollowPoint.followPoint;
        }
        else
        {
            _doNotFollow = true;
        }
    }

    void FixedUpdate()
    {
        if (_doNotFollow) return;

        transform.position = _followPoint.position;
    }
}