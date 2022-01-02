using System.Collections;
using UnityEngine;

public class InterfaceSceneManager : MonoBehaviour
{
    private static InterfaceSceneManager _instance;
    private static bool _initialized;

    private States _currentState = States.GameView;

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
            var tileClicker = TileClicker.Get();
            if (!tileClicker.Frozen())
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
        GUIScene.Get().Hide();
        MenuScene.Get().Show();
        _currentState = States.MenuScene;
    }

    private void HideCurrentView()
    {
        if (_currentState == States.BonfireScene)
        {
            TileClicker.Get().Freeze(.1f);
            BonfireScene.Get().Hide();

            ShowGameView();
        }
        else if (_currentState == States.MenuScene)
        {
            TileClicker.Get().Freeze(.1f);
            MenuScene.Get().Hide();
            
            ShowGameView();
        }
    }

    public void ShowBonfireScene()
    {
        TileClicker.Get().Disable();
        GUIScene.Get().Hide();
        BonfireScene.Get().Show();

        _currentState = States.BonfireScene;
    }
}