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
        if (Random.value < .001f)
        {
            UpdateShrineCount();
        }

        if (_shrineCount > 0) return;

        if (Random.value < .02f)
        {
            _spawnShrine = true;
        }

        if (_spawnShrine && CanSpawnShrineThisFrame())
        {
            TrySpawnShrineAtRandomGrassBlock();
        }
    }

    private void TrySpawnShrineAtRandomGrassBlock()
    {
        _spawnShrine = false;
        
        _blocksWithGreens = _worldPlane
            .GetBlocksWithGreens()
            .ToList();

        var block = _blocksWithGreens[Random.Range(0, _blocksWithGreens.Count)];

        var greensNearby = _worldPlane
            .GetNearbyBlocksWithinRange(block.GetGridPosition(), 5)
            .Count(otherBlock => otherBlock.OccupiedByGrownGreens());

        if (greensNearby > 60)
        {
            SpawnShrine(block);
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

    private void UpdateShrineCount()
    {
        _shrineCount = _worldPlane.GetBlocksWithShrines().Count();
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