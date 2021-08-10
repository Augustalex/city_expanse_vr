using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTouch : MonoBehaviour
{
    void Start()
    {
        if (TouchGlobals.usingTouch)
        {
            gameObject.SetActive(false);
        }
    }
}
