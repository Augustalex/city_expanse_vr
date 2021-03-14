using System;
using UnityEngine;

public class WorkQueue : MonoBehaviour
{
    public static WorkQueue instance;
    private int _nextTicket = 1;
    private int _currentTicket = 0;

    public static WorkQueue Get()
    {
        return instance;
    }

    public void Awake()
    {
        instance = this;
    }

    void Update()
    {
        _currentTicket = Math.Min(_currentTicket + 1, _nextTicket);
    }

    public int GetTicket()
    {
        return _nextTicket++;
    }

    public bool HasTicketForFrame(int ticket)
    {
        return ticket == _currentTicket;
    }
    
    public bool HasExpiredTicket(int ticket)
    {
        return ticket < _currentTicket;
    }
}