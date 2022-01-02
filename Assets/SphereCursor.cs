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
    private TileClicker _tileClicker;
    private Animator _animator;
    private static readonly int Small = Animator.StringToHash("Small");
    private Block _activeBlock;

    private const float SpeedModifier = 25f;

    void Start()
    {
        _camera = CameraManager.PrimaryCamera();
        _sphereCursorMesh = GetComponentInChildren<SphereCursorMesh>().gameObject;
        _tileClicker = TileClicker.Get();

        _animator = _sphereCursorMesh.GetComponent<Animator>();
    }

    void Update()
    {
        var moveInstantly = _tileClicker.GetInteractionDuration() > 1f;
        if (moveInstantly && !_animator.GetBool(Small))
        {
            _animator.SetBool(Small, true);
        }
        else if (!moveInstantly && _animator.GetBool(Small))
        {
            _animator.SetBool(Small, false);
        }

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
                _speed = (_target - transform.position).magnitude * SpeedModifier;

                var isActive = _sphereCursorMesh.gameObject.activeSelf;
                if (!isActive)
                {
                    _sphereCursorMesh.gameObject.SetActive(true);
                    transform.position = _target;
                }
                else if (moveInstantly)
                {
                    _speed = (_target - transform.position).magnitude * SpeedModifier * .75f;
                }

                _activeBlock = block;
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

    public Block GetSelectedBlock()
    {
        return _activeBlock;
    }
}