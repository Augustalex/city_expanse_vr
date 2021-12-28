using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockCapacityText : MonoBehaviour
{
    private TMP_Text _text;
    private BlockCapacity _blockCapacity;
    private float _alertedAt;
    private int _lastKnownCount;
    private WorldPlane _worldPlane;
    private bool _started;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        
        _blockCapacity = BlockCapacity.Get();
        _blockCapacity.OnCountChange += CountChanged;

        _text.text = "";
        _started = false;
    }

    private void CountChanged(int newCount)
    {
        _lastKnownCount = newCount;
        
        // if (_lastKnownCount == _blockCapacity.MaxCapacity)
        // {
        //     _text.text = _lastKnownCount + "/<color=red>" + _blockCapacity.MaxCapacity + "</color>";
        //     _alertedAt = Time.time;
        // }
        if (_lastKnownCount == 0)
        {
            // _text.text = "<color=red>" + _lastKnownCount + "</color>/" + _blockCapacity.MaxCapacity;
            _text.text = "<color=red>" + _lastKnownCount + "</color>";
            _alertedAt = Time.time;
        }
        else
        {
            _text.text = _lastKnownCount.ToString();
        }
    }

    void Update()
    {
        if (!_worldPlane.WorldGenerationDone())
        {
            _text.text = "";
            return;
        }

        if (!_started)
        {
            _started = true;
            CountChanged(_blockCapacity.DefaultCount);
        }
        
        var timeSinceLastAlert = Time.time - _alertedAt;
        if (timeSinceLastAlert > 5)
        {
            // _text.text = _lastKnownCount + "/" + _blockCapacity.MaxCapacity;
            _text.text = _lastKnownCount.ToString();
        }
    }
}