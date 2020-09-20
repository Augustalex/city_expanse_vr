using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CItyForrestShrine : MonoBehaviour
{
    public GameObject shrineTemplate;
    
    private WorldPlane _worldPlane;
    
    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        if (Random.value < .1f)
        {
            var shrineCount = _worldPlane.GetBlocksWithShrines().Count();
            if (shrineCount > 0) return;

            var suitableLocation = _worldPlane
                .GetBlocksWithGreens()
                .Where(block =>
                {
                    var greensNearby = _worldPlane
                        .GetNearbyBlocksWithinRange(block.GetGridPosition(), 5)
                        .Count(otherBlock => otherBlock.OccupiedByGrownGreens());
                    return greensNearby > 60;
                })
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

            if (suitableLocation != null)
            {
                SpawnShrine(suitableLocation);
            }
        }
    }

    private void SpawnShrine(Block suitableLocation)
    {
        var shrine = Instantiate(shrineTemplate);
        suitableLocation.DestroyOccupant();
        suitableLocation.Occupy(shrine);
    }
}
