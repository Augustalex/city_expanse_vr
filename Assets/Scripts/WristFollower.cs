using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristFollower : MonoBehaviour
{
    public Transform followPoint;
    
    void Update()
    {
        transform.SetPositionAndRotation(followPoint.position, followPoint.rotation);
    }
}
