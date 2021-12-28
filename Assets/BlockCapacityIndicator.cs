using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCapacityIndicator : MonoBehaviour
{
    private Vector3 _originalScale;
    private BlockCapacity _blockCapacity;

    void Start()
    {
        _originalScale = transform.localScale;

        _blockCapacity = BlockCapacity.Get();
        _blockCapacity.OnCountChange += CountChanged;

        CountChanged(_blockCapacity.MaxCapacity);
    }
    private void CountChanged(int newCount)
    {
        float countForPercentage = Mathf.Max(newCount, 5);
        float percentageCapacity = countForPercentage / _blockCapacity.MaxCapacity;
        transform.localScale = _originalScale * percentageCapacity;
    }
}
