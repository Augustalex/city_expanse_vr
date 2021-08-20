using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
public class SiloSpawn : MonoBehaviour
{
    public GameObject[] siloTemplates;
    private SmokeEffect _smokeEffect;
    
    private static int _siloCount = 0;

    private void Start()
    {
        _smokeEffect = GetComponentInChildren<SmokeEffect>();
        
        var farmTemplate = siloTemplates[Random.Range(0, siloTemplates.Length)];
        var farm = Instantiate(farmTemplate, transform, false);
        
        _smokeEffect.PlayOnNextHit();
        farm.transform.Rotate(new Vector3(0, Random.value * 360, 0));
        LaunchFromAbove();

        if (_siloCount == 0)
        {
            DiscoveryScene.Get().DiscoveredSilo();
        }
        
        _siloCount += 1;
    }

    private void LaunchFromAbove()
    {
        transform.position += Vector3.up * .2f;
    }
}