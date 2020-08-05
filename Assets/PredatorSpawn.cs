using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GreensSpawn))]
[RequireComponent(typeof(BlockRelative))]
public class PredatorSpawn : MonoBehaviour
{
    public GameObject predatorTemplate;
    
    private bool _populated;
    private WorldPlane _worldPlane;
    private Block _block;

    void Start()
    {
        _worldPlane = FindObjectOfType<WorldPlane>();
        _block = GetComponent<BlockRelative>().block;
    }
    
    void Update()
    {
        if (FeatureToggles.Get().predators)
        {
            MaybePopulate();
        }
    }

    public bool IsPopulated()
    {
        return _populated;
    }

    private void Populate()
    {
        var predator = Instantiate(predatorTemplate);
        var predatorComponent = predator.GetComponent<Predator>();
        predatorComponent.SetHome(transform);
        predatorComponent.SetHomeBlock(_block);
    }
    
    public void MaybePopulate()
    {
        if (!_populated && Random.value < .01f)
        {
            var gridPosition = _block.GetGridPosition();
            var treesNearby = _worldPlane.GetNearbyBlocksWithinRange(gridPosition, 10f)
                .Count(block =>
                {
                    if (!block.OccupiedByGreens()) return false;
                    
                    var predatorSpawn = block.GetOccupantGreens().GetComponent<PredatorSpawn>();
                    if (!predatorSpawn) return false;
                    
                    return !predatorSpawn.IsPopulated();
                });

            if (treesNearby > 30)
            {
                Populate();
                _populated = true;
            }
        }   
    }
}