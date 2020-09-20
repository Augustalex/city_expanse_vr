using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CItyForrestShrine : MonoBehaviour
{
    public GameObject shrineTemplate;
    
    private WorldPlane _worldPlane;

    private const float ShrineRange = 2;
    
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
        
        UpgradeTrees(suitableLocation.GetGridPosition());
    }

    private void UpgradeTrees(Vector3 shrinePosition)
    {
        var bigTress = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 6)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();

        foreach (var treeBlock in bigTress)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Big);
        }
        
        var hugeTrees = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 4)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();

        foreach (var treeBlock in hugeTrees)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Huge);
        }
        
        var nearTrees = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 1.5f)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();

        foreach (var treeBlock in nearTrees)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Small);
        }
    }
}
