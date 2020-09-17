using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform followPoint;
    
    void Update()
    {
        transform.SetPositionAndRotation(followPoint.position, followPoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        var palette = other.GetComponent<BlockInteractionPalette>();
        if (palette)
        {
            palette.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var palette = other.GetComponent<BlockInteractionPalette>();
        if (palette)
        {
            palette.Hide();
        }
    }
}
