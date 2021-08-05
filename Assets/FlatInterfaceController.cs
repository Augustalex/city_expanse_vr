using UnityEngine;

public class FlatInterfaceController : MonoBehaviour
{
    private static FlatInterfaceController _instance;
    private Vector3 _originalScale;

    public static FlatInterfaceController Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _originalScale = transform.localScale;
        _instance = this;
    }

    public void Disable()
    {
        transform.localScale = Vector3.zero;
    }

    public void Enable()
    {
        transform.localScale = _originalScale;
    }
}