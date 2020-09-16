using System;
using UnityEngine;

public class SandSpreadController : MonoBehaviour
{
    public float chance = .003f;
    public float natureScoreRadius = 6.5f;
    public int houseCountThreshold = 10;
    public float startingThreshold = .005f;
    public float spreadResistanceThreshold = 25;
    public float spreadCombatThreshold = 30;
    public float continuationThreshold = .0001f;
    private static SandSpreadController _sandSpreadControllerInstance;

    private void Awake()
    {
        _sandSpreadControllerInstance = this;
    }

    public static SandSpreadController Get()
    {
        return _sandSpreadControllerInstance;
    }
}