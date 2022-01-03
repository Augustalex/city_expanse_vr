using System;
using System.Collections;
using UnityEngine;

public class WorldGeneratorController : MonoBehaviour
{
    public enum WorldTypes
    {
        Plains,
        SuperSmallPlains,
        Mountains
    };

    private WorldTypes _worldType = WorldTypes.Plains;
    private WorldPlaneWorldGeneratorInterface _unityInterface;
    private WorldGenerator _worldGenerator;
    private WorldPlane _worldPlane;
    private static WorldGeneratorController _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static WorldGeneratorController Get()
    {
        return _instance;
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
        _unityInterface = WorldPlaneWorldGeneratorInterface.Get();
        _worldGenerator = GetWorldGenerator();
    }

    private WorldGenerator GetWorldGenerator()
    {
        if (_worldType == WorldTypes.Plains)
        {
            return new FlatLandsWorldGenerator(_unityInterface);
        }
        else if (_worldType == WorldTypes.Mountains)
        {
            return new MountainousLandWorldGenerator(_unityInterface);
        }
        else
        {
            return new SuperSmallFlatLandsWorldGenerator(_unityInterface);
        }
    }

    public void Create(Vector2 dimensions)
    {
        _worldPlane.DestroyAllBlocks();

        _worldPlane.SetDimensions(dimensions);
        _worldPlane.SetWorldGenerationIncomplete();
        
        StartCoroutine(CreateWorldAsync());

        IEnumerator CreateWorldAsync()
        {
            yield return _worldGenerator.Create(dimensions);
            _worldPlane.SetWorldGenerationDone();
            yield return null;
        }
    }
}