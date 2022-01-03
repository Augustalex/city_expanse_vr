using System.Collections;
using UnityEngine;

public abstract class WorldGenerator
{
    protected readonly IWorldGeneratorUnityInterface UnityInterface;

    public WorldGenerator(IWorldGeneratorUnityInterface unityInterface)
    {
        UnityInterface = unityInterface;
    }

    public abstract IEnumerator Create(Vector2 dimensions);
}