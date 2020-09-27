using System;
using UnityEngine;

public class HandInteractorFollowPoint : MonoBehaviour
{
    public Transform FollowPoint;
    
    private HandInteractorFollowPoint _instance;

    private void Awake()
    {
        _instance = this;
    }

    public HandInteractorFollowPoint Get()
    {
        return _instance;
    }
}
