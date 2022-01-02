using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityForrestShrine : MonoBehaviour
{
    public GameObject shrineTemplate;

    private WorldPlane _worldPlane;
    private List<Block> _blocksWithGreens;

    private const float ShrineRange = 2;
    private int _shrineCount = 0;
    private WorkQueue _workQueue;
    private int _ticket;
    private bool _spawnShrine;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
    }

    void Update()
    {
        return; // DISABLED
        
        if (_shrineCount > 0) return;

        if (Random.value < .02f)
        {
            _spawnShrine = true;
        }

        if (_spawnShrine && CanSpawnShrineThisFrame())
        {
            _spawnShrine = false;

            TrySpawnShrineAtRandomGrassBlock();
        }
    }

    private void TrySpawnShrineAtRandomGrassBlock()
    {
        _blocksWithGreens = _worldPlane
            .GetBlocksWithGreens()
            .ToList();

        var count = _blocksWithGreens.Count;
        if (count > 0)
        {
            var block = _blocksWithGreens[Random.Range(0, count)];

            var greensNearby = _worldPlane
                .GetNearbyBlocksWithinRange(block.GetGridPosition(), 8)
                .Count(otherBlock => otherBlock.OccupiedByGrownGreens());

            if (greensNearby > 80)
            {
                SpawnShrine(block);
            }
        }
    }

    private bool CanSpawnShrineThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private void SpawnShrine(Block suitableLocation)
    {
        var shrine = Instantiate(shrineTemplate);
        suitableLocation.DestroyOccupant();
        suitableLocation.Occupy(shrine);

        UpgradeTrees(suitableLocation.GetGridPosition());
        
        _shrineCount += 1;
    }

    private void UpgradeTrees(Vector3 shrinePosition)
    {
        var bigTress = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 10)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();

        foreach (var treeBlock in bigTress)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Big);
        }

        var hugeTrees = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 6)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();

        foreach (var treeBlock in hugeTrees)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Small);
        }

        var nearTrees = _worldPlane
            .GetNearbyBlocksWithinRange(shrinePosition, 2f)
            .Where(block => block.OccupiedByGrownGreens())
            .ToList();
        foreach (var treeBlock in nearTrees)
        {
            treeBlock.GetOccupantGreens().SetSize(GreensSpawn.TreeSize.Tiny);
        }
    }
}