using System;
using UnityEngine;

public class BonfireStateManager : MonoBehaviour
{
    private int _devotees = 0;
    private static BonfireStateManager _instance;
    
    private void Awake()
    {
        _instance = this;
    }

    public static BonfireStateManager Get()
    {
        return _instance;
    }
    
    public void SetDevotees(int count)
    {
        _devotees = count;
    }

    public int GetDevoteeCount()
    {
        return _devotees;
    }
}