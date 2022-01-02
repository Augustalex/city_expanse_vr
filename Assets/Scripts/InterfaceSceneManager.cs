using System.Collections;
using UnityEngine;

public class InterfaceSceneManager : MonoBehaviour
{
    private static InterfaceSceneManager _instance;
    private static bool _initialized;

    private States _currentState = States.GameView;

    private float _lastOpenedModal = 0;

    private enum States
    {
        BonfireScene,
        GameView,
        MenuScene
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        HideAll();
        ShowGameView();
    }

    private void ShowGameView()
    {
        GUIScene.Get().Show();
        _currentState = States.GameView;

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForEndOfFrame();
            TileClicker.Get().Enable();
        }
    }

    private void HideAll()
    {
        GUIScene.Get().Hide();
        BonfireScene.Get().Hide();
    }

    public static InterfaceSceneManager Get()
    {
        return _instance;
    }

    void Update()
    {
        if (_currentState != States.GameView)
        {
            if (!Frozen())
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
                {
                    TileClicker.Get().Freeze(.1f);
                    HideCurrentView();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowMenuScene();
            }
        }
    }

    private void ShowMenuScene()
    {
        TileClicker.Get().Disable();
        _lastOpenedModal = Time.time;
        
        GUIScene.Get().Hide();
        MenuScene.Get().Show();
        _currentState = States.MenuScene;
    }

    private bool Frozen()
    {
        return Time.time - _lastOpenedModal < .5f;
    }

    private void HideCurrentView()
    {
        TileClicker.Get().Freeze(.1f);
        
        if (_currentState == States.BonfireScene)
        {
            BonfireScene.Get().Hide();
        }
        else if (_currentState == States.MenuScene)
        {
            MenuScene.Get().Hide(); 
        }

        ShowGameView();
    }

    public void ShowBonfireScene()
    {
        TileClicker.Get().Disable();
        _lastOpenedModal = Time.time;
        
        GUIScene.Get().Hide();
        BonfireScene.Get().Show();

        _currentState = States.BonfireScene;
    }
}