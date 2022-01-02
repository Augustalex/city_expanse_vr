using UnityEngine;

public class PlaceBonfireCursorAction : CursorAction
{
    public GameObject bonfireTemplate;

    public override bool PerformsOnce()
    {
        return true;
    }

    public override void Perform(Block cursorPosition)
    {
        var bonfire = Instantiate(bonfireTemplate);
        cursorPosition.Occupy(bonfire);
    }

    public override bool CanPerform(Block cursorPosition)
    {
        return cursorPosition.IsGrass();
    }
}