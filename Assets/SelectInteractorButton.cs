using UnityEngine;

public class SelectInteractorButton : MonoBehaviour
{
    public InteractorHolder interactorHolder;
    public InteractorHolder.BlockInteractors interactorType;

    public void OnClick()
    {
        interactorHolder.SetInteractor(interactorType);
    }
}
