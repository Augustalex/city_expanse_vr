using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMover : MonoBehaviour
{
    void Update()
    {
        var direction = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
             direction = transform.forward * Time.deltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = transform.forward * (Time.deltaTime * -1f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = transform.right * (Time.deltaTime * -1f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = transform.right * Time.deltaTime;
        }

        transform.position += direction.normalized * (Time.deltaTime * 10f);
    }
}
