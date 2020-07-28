using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject target;

    void FixedUpdate()
    {
        if (target)
        {
            transform.position = target.transform.position;
        }
    }
}
