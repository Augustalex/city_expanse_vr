using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Transform _followPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_followPoint)
        {
            var newPosition = new Vector3(_followPoint.position.x, transform.position.y, _followPoint.position.z);
            transform.position = newPosition;
        }
    }

    public void Hook(Transform followPoint)
    {
        _followPoint = followPoint;
    }

    public void UnHook(Transform referenceFollowPoint)
    {
        if (_followPoint == referenceFollowPoint)
        {
            _followPoint = null;
            var direction = Camera.main.transform.forward.normalized;
            GetComponent<Rigidbody>().AddForce(direction * 1f, ForceMode.Impulse);
        }
    }
}