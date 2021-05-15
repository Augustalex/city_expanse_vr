using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    private Camera _camera;
    private bool _rightDown;
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
        var change = Input.mouseScrollDelta.y;
        _camera.transform.position += new Vector3(0, change * .05f, 0);

        var cameraYMax = 5f;
        var cameraYMin = 1.4f;

        var cameraXMin = -2f;
        var cameraXMax = 4f;
        
        var cameraZMin = -3;
        var cameraZMax = 3;

        if (change > 0 || change < 0)
        {
            var newYPosition = _camera.transform.position.y;
            var zoomProgress = Mathf.Clamp((newYPosition - cameraYMin) / (cameraYMax - cameraYMin), 0f, 1f);
            var easedZoomProgress = EaseOutExpo(zoomProgress);

            var minTilt = 4f;
            var maxTilt = 90f;
            var newTilt = (maxTilt - minTilt) * easedZoomProgress + minTilt;
            var currentTilt = _camera.transform.rotation.eulerAngles.x;
            var differenceOfTilt = newTilt - currentTilt;
            if (currentTilt + differenceOfTilt <= maxTilt || currentTilt + differenceOfTilt >= minTilt)
            {
                _camera.transform.RotateAround(_camera.transform.position, _camera.transform.right, differenceOfTilt);
                     
            }
        }


        const float cameraMovementSpeed = .75f;
        var direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.back;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.forward;
        }

        _camera.transform.position += direction * (cameraMovementSpeed * Time.deltaTime);
        
        // var rotation = 0;
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     rotation -= 1;
        // }
        // else if (Input.GetKey(KeyCode.E))
        // {
        //     rotation += 1;
        // }
        //
        // _camera.transform.RotateAround(_camera.transform.position, Vector3.up, rotation * 40 * Time.deltaTime);

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
                var movementDelta = (endPosition - _startLeftMouseButtonMovePosition);

                var progress = movementDelta.magnitude / (_startLeftMouseButtonMovePosition + new Vector3(250, 0, 250) -
                                                          _startLeftMouseButtonMovePosition).magnitude;
                var curvedProgress = progress;
                var speed = .01f * Mathf.Clamp(curvedProgress, 0, 1);

                _camera.transform.position += new Vector3(movementDelta.y, 0, -movementDelta.x).normalized * -speed;
            }

            _leftDown = true;
        }

        if (!Input.GetMouseButton(0))
        {
            _leftDown = false;
        }

        if (Input.GetMouseButton(1))
        {
            if (!_rightDown)
            {
                _startCameraPosition = _camera.transform.position;
                _startMovePosition = Input.mousePosition;
                _camera.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                var endPosition = Input.mousePosition;
                var movementDelta = (endPosition - _startMovePosition);

                var progress = movementDelta.magnitude /
                               (_startMovePosition + new Vector3(250, 0, 250) - _startMovePosition).magnitude;
                var curvedProgress = QuarticEaseIn(progress);
                var speed = .05f * Mathf.Clamp(curvedProgress, 0, 1);


                // _camera.transform.position = _startCameraPosition + _camera.transform.forward * (movementDelta.y * -speed) +
                                             // _camera.transform.right * (-movementDelta.x * -speed);  
                _camera.transform.position = _startCameraPosition +
                                             new Vector3(movementDelta.y, 0, -movementDelta.x).normalized * -speed;
            }

            _rightDown = true;
        }
        else
        {
            if (_rightDown)
            {
                var endPosition = Input.mousePosition;
                var movementDelta = (endPosition - _startMovePosition);

                var progress = movementDelta.magnitude /
                               (_startMovePosition + new Vector3(250, 0, 250) - _startMovePosition).magnitude;
                var curvedProgress = progress;
                var speed = .02f * Mathf.Clamp(curvedProgress, 0, 1);

                _camera.GetComponent<Rigidbody>().AddForce(new Vector3(movementDelta.y, 0, -movementDelta.x) * speed,
                    ForceMode.Impulse);
            }

            _rightDown = false;
        }

        var newPosition = _camera.transform.position;
        _camera.transform.position = new Vector3(
            Mathf.Clamp(newPosition.x, cameraXMin, cameraXMax),
            Mathf.Clamp(newPosition.y, cameraYMin, cameraYMax),
            Mathf.Clamp(newPosition.z, cameraZMin, cameraZMax)
        );
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