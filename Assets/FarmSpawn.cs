using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
public class FarmSpawn : MonoBehaviour
{
    public GameObject[] farmTemplates;
    private FarmState _state = FarmState.Dead;
    private SmokeEffect _smokeEffect;

    private enum FarmState
    {
        Dead,
        FullyGrown
    }

    private void Start()
    {
        _smokeEffect = GetComponentInChildren<SmokeEffect>();
    }

    public void Grow()
    {
        if (_state == FarmState.FullyGrown) return;
        
        var farmTemplate = farmTemplates[Random.Range(0, farmTemplates.Length)];
        var farm = Instantiate(farmTemplate, transform, false);
        
        _smokeEffect.PlayOnNextHit();
        farm.transform.Rotate(new Vector3(0, Random.value * 360, 0));
        LaunchFromAbove();

        _state = FarmState.FullyGrown;
    }
    
    private void LaunchFromAbove()
    {
        transform.position += Vector3.up * .2f;
    }
}
