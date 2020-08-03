using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WorldPlane))]
public class GameObjective : MonoBehaviour
{
    private enum Objective
    {
        OneHouse,
        OneBigHouse,
        OneMegaHouse
    }

    private Objective _currentObjective = Objective.OneMegaHouse;
    private WorldPlane _worldPlane;
    private bool _initialized;

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
    }

    public bool Reached()
    {
        if (_currentObjective == Objective.OneHouse)
        {
            return _worldPlane.GetBlocksWithHouses().Count > 0;
        }
        else if (_currentObjective == Objective.OneBigHouse)
        {
            return _worldPlane
                .GetBlocksWithHouses()
                .Select(block => block.GetOccupantHouse())
                .Count(house => house.IsBig()) > 0;
        }
        else if (_currentObjective == Objective.OneMegaHouse)
        {
            return _worldPlane
                .GetBlocksWithHouses()
                .Select(block => block.GetOccupantHouse())
                .Count(house => house.IsMegaBig()) > 0;
        }
        else
        {
            return false;
        }
    }

    public void Progress()
    {
        _currentObjective = NextObjective();
        
        Reset();
    }

    public void Reset()
    {
        _worldPlane.ResetAtSize(WorldSizeOfObjective());
    }

    private WorldPlane.Size WorldSizeOfObjective()
    {
        switch (_currentObjective)
        {
            case Objective.OneHouse:
                return WorldPlane.Size.Small;
            case Objective.OneBigHouse:
                return WorldPlane.Size.Medium;
            case Objective.OneMegaHouse:
                return WorldPlane.Size.Large;
            default:
                throw new Exception("No specified world size for current objective");
        }
    }

    private Objective NextObjective()
    {
        switch (_currentObjective)
        {
            case Objective.OneHouse:
                return Objective.OneBigHouse;
            case Objective.OneBigHouse:
                return Objective.OneMegaHouse;
            case Objective.OneMegaHouse:
                return Objective.OneMegaHouse;
            default:
                return Objective.OneHouse;
        }
    }

    public bool Initialized()
    {
        return _initialized;
    }

    public void Initialize()
    {
        _initialized = true;
        
        Reset();
    }
}