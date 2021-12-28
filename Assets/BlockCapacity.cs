using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCapacity : MonoBehaviour
{
    private static BlockCapacity _instance;
    public static int DefaultCount = 20;
    public readonly int MaxCapacity = 20;
    private int _count = 0;
    public event Action<int> OnCountChange;

    public static BlockCapacity Get()
    {
        return _instance;
    }
    
    private void Awake()
    {
        _instance = this;
        _count = DefaultCount;
    }

    public void IncreaseCount(int amount = 1)
    {
        _count = Mathf.Min(_count + 1, DefaultCount);
        CountChange(_count);
    }

    public void DecreaseCount(int amount = 1)
    {
        _count = Mathf.Max(_count - 1, 0);
        
        CountChange(_count);
    }
    
    public bool HasBlocks()
    {
        return _count > 0;
    }

    public bool HasMoreCapacity()
    {
        return _count < MaxCapacity;
    }

    protected virtual void CountChange(int newCount)
    {
        OnCountChange?.Invoke(newCount);
    }
}
