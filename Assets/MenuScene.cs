using UnityEngine;

public class MenuScene : MonoBehaviour
{
    private GameObject _camera;
    private GameObject _trophiesRoot;
    private Trophies _trophies;
    private bool _shown = false;
    private static MenuScene _instance;

    public static MenuScene Get()
    {
        return _instance;
    }
    
    private void Awake()
    {
        _camera = GetComponentInChildren<MenuCamera>().gameObject;
        _trophies = GetComponentInChildren<Trophies>();
        _trophiesRoot = _trophies.gameObject;
        
        _instance = this;
    }

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        _shown = true;
        
        _camera.SetActive(true);
        _trophiesRoot.SetActive(true);
        _trophies.Refresh();
    }

    public void Hide()
    {
        _camera.SetActive(false);
        _trophiesRoot.SetActive(false);

        _shown = false;
    }

    public bool IsShowing()
    {
        return _shown;
    }
}