using System;
using UnityEngine;

public class HandInteractorFollowPoint : MonoBehaviour
{
    public Transform followPoint;
    
    private static HandInteractorFollowPoint _instance;

    private void Awake()
    {
        _instance = this;
        if (followPoint == null)
        {
            Debug.LogError("No follow point was set as Main Hand Interactor!");
        }
    }

    public static HandInteractorFollowPoint Get()
    {
        return _instance;
    }
}
