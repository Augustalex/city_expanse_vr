using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public class SphereCursor : MonoBehaviour
{
    private Camera _camera;
    private GameObject _sphereCursorMesh;

    private Vector3 _target = Vector3.zero;
    private float _speed;

    void Start()
    {
        _camera = FindObjectOfType<Camera>();
        _sphereCursorMesh = GetComponentInChildren<SphereCursorMesh>().gameObject;
    }

    void Update()
    {
        if (CameraMover.IsRotating())
        {
            _sphereCursorMesh.gameObject.SetActive(false);
        }
        else
        {
            if (_target != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            }
        
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(GetPointerPosition());
            if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

            var block = hit.collider.GetComponentInParent<Block>();
            if (block)
            {
                _target = block.transform.position + Vector3.up * 0.05f;
                _speed = (_target - transform.position).magnitude * 35f;
                
                if (!_sphereCursorMesh.gameObject.activeSelf)
                {
                    _sphereCursorMesh.gameObject.SetActive(true);
                    transform.position = _target;
                }
            }
            else
            {
                _sphereCursorMesh.gameObject.SetActive(false);
            }   
        }
    }
    
    private Vector2 GetPointerPosition()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2) Input.mousePosition;
    }
}
