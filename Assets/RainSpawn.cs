using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RainSpawn : MonoBehaviour
{
    public GameObject dropletTemplate;
    public GameObject stormTemplate;

    private bool _storm = true;
    private CloudStyleControl _cloudStyleControl;

    private void Awake()
    {
        _cloudStyleControl = GetComponentInParent<CloudStyleControl>();
    }

    void Update()
    {
        if (Random.value < .001f)
        {
            _storm = !_storm;
            if (_storm)
            {
                _cloudStyleControl.SetStormMaterial();
            }
            else
            {
                _cloudStyleControl.SetNormalMaterial();
            }
        }
        
        if (_storm)
        {
            SpawnStormDroplet(RandomLocationInArea());
        }
        else
        {
            if (Random.value < .5f)
            {
                SpawnDroplet(RandomLocationInArea());
            }
        }
    }

    private void SpawnStormDroplet(Vector3 spawnPosition)
    {
        Instantiate(stormTemplate, spawnPosition, Quaternion.identity);
    }

    private void SpawnDroplet(Vector3 spawnPosition)
    {
        Instantiate(dropletTemplate, spawnPosition, Quaternion.identity);
    }

    private Vector3 RandomLocationInArea()
    {
        var position = transform.position;
        var scale = transform.localScale;
        var originalX = position.x;
        var originalZ = position.z;

        var minX = originalX - scale.x * .5f;
        var maxX = originalX + scale.x * .5f;
        var minZ = originalZ - scale.z * .5f;
        var maxZ = originalZ + scale.z * .5f;

        var x = Random.Range(minX, maxX);
        var z = Random.Range(minZ, maxZ);

        return new Vector3(x, position.y, z);
    }
}