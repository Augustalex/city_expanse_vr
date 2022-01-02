using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCursor))]
[RequireComponent(typeof(PlaceBonfireCursorAction))]
public class SphereCursorAction : MonoBehaviour
{
    private bool _hasActiveAction = false;
    private CursorAction _currentAction;
    private SphereCursor _cursor;

    public enum CursorActions
    {
        PlaceBonfire
    }

    public void SetCursorAction(CursorActions cursorAction)
    {
        if (cursorAction == CursorActions.PlaceBonfire)
        {
            SetCursorAction(GetComponent<PlaceBonfireCursorAction>());
        }
    }

    private void SetCursorAction(CursorAction cursorAction)
    {
        _currentAction = cursorAction;
        _hasActiveAction = true;
    }

    public bool HasActiveAction()
    {
        return _hasActiveAction;
    }

    public void Perform()
    {
        _cursor = GetComponent<SphereCursor>();
        var cursorPosition = _cursor.GetSelectedBlock();
        if (_currentAction.CanPerform(cursorPosition))
        {
            _currentAction.Perform(cursorPosition);
            if (_currentAction.PerformsOnce())
            {
                _hasActiveAction = false;
                _currentAction = null;
            }
        }
    }
}