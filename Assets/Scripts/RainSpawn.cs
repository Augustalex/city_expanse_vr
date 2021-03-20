using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RainSpawn : MonoBehaviour
{
    public GameObject dropletTemplate;
    public GameObject stormTemplate;

    private CloudStyleControl _cloudStyleControl;
    private readonly Queue<GameObject> _droplets = new Queue<GameObject>();
    private FeatureToggles _featureToggles;
    private CloudState _cloudState = CloudState.Normal;

    private enum CloudState
    {
        Normal,
        Storm
    }

    private void Awake()
    {
        _cloudStyleControl = GetComponentInParent<CloudStyleControl>();

        _cloudStyleControl.SetNormalMaterial();
    }

    private void Start()
    {
        _featureToggles = FeatureToggles.Get();
    }

    void Update()
    {
        if (_droplets.Count > 10)
        {
            var dropletToKill = _droplets.Dequeue();
            Destroy(dropletToKill);
        }

        if (_featureToggles.storms && Random.value < .001f) ToggleCloudState();

        SpawnDroplet();
    }

    private void ToggleCloudState()
    {
        _cloudState = _cloudState == CloudState.Normal ? CloudState.Storm : CloudState.Normal;

        if (_cloudState == CloudState.Storm)
        {
            _cloudStyleControl.SetStormMaterial();
        }
        else
        {
            _cloudStyleControl.SetNormalMaterial();
        }
    }

    private void SpawnDroplet()
    {
        if (_cloudState == CloudState.Storm && Random.value < .5f)
        {
            SpawnStormDroplet(RandomLocationInArea());
        }
        else if (_cloudState == CloudState.Normal && Random.value < .5f)
        {
            SpawnDroplet(RandomLocationInArea());
        }
    }

    private void SpawnStormDroplet(Vector3 spawnPosition)
    {
        Instantiate(stormTemplate, spawnPosition, Quaternion.identity);
    }

    private void SpawnDroplet(Vector3 spawnPosition)
    {
        var droplet = Instantiate(dropletTemplate, spawnPosition, Quaternion.identity);
        _droplets.Enqueue(droplet);
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