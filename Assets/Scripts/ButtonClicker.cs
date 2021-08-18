using System;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    public Camera mainCamera;

    private InteractorHolder _interactorHolder;
    private bool _coolingDown;
    private float _cooldownTimeLeft;
    private float _lastLayer = 999f;
    private CameraMover _cameraMover;

    void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
        _cameraMover = GetComponent<CameraMover>();
    }

    void Update()
    {
        if (GetPointerDown())
        {
            if (!_interactorHolder.AnyInteractorActive() && !_cameraMover.IsTouchZooming())
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(GetPointerPosition());
                if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

                var isZoomIn = hit.collider.GetComponent<ZoomIn>();
                var isZoomOut = hit.collider.GetComponent<ZoomOut>();
                if (isZoomIn || isZoomOut)
                {
                    var change = isZoomOut ? 15f * Time.deltaTime : -15f * Time.deltaTime;
                    CameraMover.Get().Zoom(change);
                }
            }
        }
    }

    private Vector2 GetPointerPosition()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2) Input.mousePosition;
    }

    private bool GetPointerDown()
    {
        return Input.GetMouseButton(0) || (Input.touchCount == 1);
    }
}