using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    private const float CameraYMax = 5f;
    private const float CameraYMin = 1.6f;
    private const float CameraXMin = -2f;
    private const float CameraXMax = 4f;
    private const int CameraZMin = -3;
    private const int CameraZMax = 3;
    private Camera _camera;
    private bool _shouldFireCameraInTargetDirection;
    private Vector3 _startMovePosition;
    private Vector3 _startCameraPosition;
    private bool _leftDown;
    private Vector3 _startLeftMouseButtonMovePosition;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        HandleZoomTilt();
        HandleKeyboardMovement();
        HandleMousePanMovement();
        HandleRightDragMovement();
        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        var newPosition = _camera.transform.position;
        _camera.transform.position = new Vector3(
            Mathf.Clamp(newPosition.x, CameraXMin, CameraXMax),
            Mathf.Clamp(newPosition.y, CameraYMin, CameraYMax),
            Mathf.Clamp(newPosition.z, CameraZMin, CameraZMax)
        );        
    }

    private void HandleZoomTilt()
    {
        var change = Input.mouseScrollDelta.y;
        _camera.transform.position += new Vector3(0, change * .1f, 0);

        if (change > 0 || change < 0)
        {
            var newYPosition = _camera.transform.position.y;
            var zoomProgress = Mathf.Clamp((newYPosition - CameraYMin) / (CameraYMax - CameraYMin), 0f, 1f);
            var easedZoomProgress = EaseOutExpo(zoomProgress);

            var minTilt = 12f;
            var maxTilt = 90f;
            var newTilt = (maxTilt - minTilt) * easedZoomProgress + minTilt;
            var currentTilt = _camera.transform.rotation.eulerAngles.x;
            var differenceOfTilt = newTilt - currentTilt;
            if (currentTilt + differenceOfTilt <= maxTilt || currentTilt + differenceOfTilt >= minTilt)
            {
                _camera.transform.RotateAround(_camera.transform.position, _camera.transform.right, differenceOfTilt);
            }
        }
    }
    
    private void HandleMousePanMovement()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0))
        {
            if (!_leftDown)
            {
                _startLeftMouseButtonMovePosition = Input.mousePosition;
                _camera.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                var endPosition = Input.mousePosition;
                var mouseDirection = (endPosition - _startLeftMouseButtonMovePosition).normalized;

                var curvedProgress = (endPosition - _startLeftMouseButtonMovePosition).magnitude / 250;
                var speed = .025f * Mathf.Clamp(curvedProgress, 0, 1);
                    
                var newDirection = _camera.transform.rotation * mouseDirection * speed;
                newDirection.y = 0;
                
                _camera.transform.position += newDirection;
            }
        
            _leftDown = true;
        }
        
        if (!Input.GetMouseButton(0))
        {
            _leftDown = false;
        }
    }

    private void HandleKeyboardMovement()
    {
        const float cameraMovementSpeed = .75f;
        var direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += _camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += -_camera.transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -_camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += _camera.transform.right;
        }
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        _camera.transform.position += direction * (cameraMovementSpeed * Time.deltaTime);
        
        var rotation = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotation -= 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotation += 1;
        }
        _camera.transform.RotateAround(_camera.transform.position, Vector3.up, rotation * 40 * Time.deltaTime);
    }

    private void HandleRightDragMovement()
    {
        if (Input.GetMouseButton(1))
        {
            if (!_shouldFireCameraInTargetDirection)
            {
                _startCameraPosition = _camera.transform.position;
                _startMovePosition = Input.mousePosition;
                _camera.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                var endPosition = Input.mousePosition;
                var mouseDirection = (endPosition - _startMovePosition).normalized;
                
                var easedVelocity = QuarticEaseIn((endPosition - _startMovePosition).magnitude / 250);
                var speed = .25f * Mathf.Clamp(easedVelocity, 0, 1);

                var newDirection = _camera.transform.rotation * mouseDirection * speed;
                newDirection.y = 0;
                
                _camera.transform.position = _startCameraPosition + newDirection * -speed;
            }

            _shouldFireCameraInTargetDirection = true;
        }
        else
        {
            if (_shouldFireCameraInTargetDirection)
            {
                var endPosition = Input.mousePosition;
                var mouseDirection = (endPosition - _startMovePosition).normalized;
                
                var easedVelocity = (endPosition - _startMovePosition).magnitude / 250;
                var speed = 2 * Mathf.Clamp(easedVelocity, 0, 1);

                var newDirection = _camera.transform.rotation * -mouseDirection * speed;
                newDirection.y = 0;
                
                _camera.GetComponent<Rigidbody>().AddForce( newDirection, ForceMode.Impulse); 
            }

            _shouldFireCameraInTargetDirection = false;
        }
    }

    private float EaseOutExpo(float x)
    {
        return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }

    public float QuarticEaseIn(float p)
    {
        return p * p * p * p;
    }

    public float QuarticEaseOut(float p)
    {
        float f = (p - 1);
        return f * f * f * (1 - p) + 1;
    }
}