using UnityEngine;

public class PuzzleSpawnUtils : MonoBehaviour
{
    public AudioClip notPossibleSound;
    
    private static PuzzleSpawnUtils _instance;
    private WorldPlane _worldPlane;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    public static PuzzleSpawnUtils Get()
    {
        return _instance;
    }

    public void PlayNotPossibleSound(Vector3 spawnGridPosition)
    {
        AudioSource.PlayClipAtPoint(notPossibleSound, _worldPlane.ToRealCoordinates(spawnGridPosition),
            .02f * GameManager.MasterVolume);
    }
}