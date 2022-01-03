using System;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject fireBallTemplate;

    public event Action BeforeDestroy;
    public event Action Hit;


    private Vector3 _target;
    private float _startTime;
    private float _duration = 12;
    private bool _started = false;
    private Vector3 _startingPosition;
    private bool _landed;
    private bool _fireBallStarted;
    private GameObject _fireBall;
    private WorldPlane _worldPlane;
    private bool _isIntro;

    public void SetDuration(float newDuration)
    {
        _duration = newDuration;
    }

    public void SetToIntroMode()
    {
        _isIntro = true;
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    private void Update()
    {
        if (_started)
        {
            var progress = Mathf.Clamp((Time.fixedTime - _startTime) / _duration, 0, 1.5f);
            transform.position = Vector3.Lerp(_startingPosition, _target, progress);

            if (progress > 1)
            {
                _landed = true;
            }
        }

        if (_landed && !_fireBallStarted)
        {
            StartFireBall();
            _fireBallStarted = true;

            OnHit();
        }
    }

    private void StartFireBall()
    {
        var fireBall = Instantiate(fireBallTemplate);
        fireBall.transform.position = transform.position;

        _fireBall = fireBall;

        var fireBallCompoent = _fireBall.GetComponent<FireBall>();
        fireBallCompoent.BeforeDestroy += ResetWorld;
        fireBallCompoent.Expand += DestroyBlocksInsideFireBall;
    }

    private void DestroyBlocksInsideFireBall()
    {
        if (_isIntro) return;
        
        var hits = Physics.OverlapSphere(_fireBall.transform.position, _fireBall.transform.localScale.x * .5f);

        foreach (var hit in hits)
        {
            var blockRigidbody = hit.attachedRigidbody;
            if (blockRigidbody)
            {
                var block = blockRigidbody.gameObject.GetComponent<Block>();
                if (block)
                {
                    _worldPlane.RemoveAndDestroyBlock(block);
                }
            }
        }
    }

    private void ResetWorld()
    {
        if (!_isIntro)
        {
            WorldGeneratorController.Get().Create(WorldPlane.SizeToDimensions(WorldPlane.Size.Large));
            FarmMasterController.OnResetWorld();
        }

        OnBeforeDestroy();
        Destroy(gameObject);
    }

    public void Shoot(Vector3 shootFrom)
    {
        if (_started) return;

        ResetPosition(shootFrom);

        _startTime = Time.fixedTime;
        _startingPosition = transform.position;
        _started = true;
    }

    private void ResetPosition(Vector3 position)
    {
        transform.LookAt(_target);
        transform.position = position + transform.forward * 1f;
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    private void OnBeforeDestroy()
    {
        BeforeDestroy?.Invoke();
    }

    protected virtual void OnHit()
    {
        Hit?.Invoke();
    }
}