using UnityEngine;

[RequireComponent(typeof(GameObjective))]
[RequireComponent(typeof(WorldPlane))]
public class GameManager : MonoBehaviour
{
    private GameObjective _gameObjective;

    void Start()
    {
        _gameObjective = GetComponent<GameObjective>();
    }

    void Update()
    {
        if (!_gameObjective.Initialized())
        {
            _gameObjective.Initialize();
        }
        else if (_gameObjective.Reached())
        {
            _gameObjective.Progress();
        }
    }
}