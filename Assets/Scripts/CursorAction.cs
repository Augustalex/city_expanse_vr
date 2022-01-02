using UnityEngine;

public abstract class CursorAction : MonoBehaviour
{
    public abstract bool PerformsOnce();
    public abstract void Perform(Block cursorPosition);

    public abstract bool CanPerform(Block cursorPosition);
}