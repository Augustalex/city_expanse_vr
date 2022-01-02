using UnityEngine;

[RequireComponent(typeof(GameObjective))]
[RequireComponent(typeof(WorldPlane))]
public class GameManager : MonoBehaviour
{
    private GameObjective _gameObjective;

    public static float MasterVolume = 10f;
    
    void Start()
    {
        _gameObjective = GetComponent<GameObjective>();
    }

    void Update()
    {
        if (!_gameObjective.Initialized())
        {
            _gameObjective.Initialize();
            FindObjectOfType<SphereCursorAction>().SetCursorAction(SphereCursorAction.CursorActions.PlaceBonfire);
        }
        else if (_gameObjective.Reached())
        {
            _gameObjective.Progress();
        }
    }
}