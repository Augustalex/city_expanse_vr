using UnityEngine;

public class RainSpawn : MonoBehaviour
{
    public GameObject dropletTemplate;
    void Update()
    {
        if (Random.value < .5f)
        {
            SpawnDroplet(RandomLocationInArea());
        }
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
