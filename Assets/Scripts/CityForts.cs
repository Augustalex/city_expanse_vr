using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityForts : MonoBehaviour
{
    public GameObject fortSpawnTemplate;

    private WorldPlane _worldPlane;
    private FeatureToggles _featureToggles;
    private int _fortCount;
    private WorkQueue _workQueue;
    private int _ticket;

    private const float HeightOfABlock = .1f;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _featureToggles = FeatureToggles.Get();
        _workQueue = WorkQueue.Get();
    }

    void Update()
    {
        if (!_featureToggles.fort) return;
        if (!CanWorkThisFrame()) return;
        
        if (_fortCount > 1) return;
        var houseCount = _worldPlane.GetBlocksWithHousesStream().Count();
        if (houseCount < 3) return;

        if (Random.value < .01f)
        {
            TrySpawnFort();
        }
    }
    
    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private void TrySpawnFort()
    {
        if (CheckIfThereAreEnoughMountainToSpawnFort()) return;

        var potentialFortBottoms = _worldPlane.GetVacantBlocksStream()
            .Where(vacantBlock =>
            {
                return vacantBlock.IsGroundLevel()
                       && _worldPlane
                           .GetMajorityBlockTypeWithinRange(vacantBlock.GetGridPosition(), 1f) == Block.BlockType.Grass
                       && _worldPlane.NoNearby(
                           vacantBlock.GetGridPosition(),
                           2f,
                           block => block.blockType == Block.BlockType.Lake
                       );
            });

        var candidates = new List<Tuple<Block, Block>>();
        foreach (var fortBottom in potentialFortBottoms)
        {
            var nearbyHighlands = _worldPlane.GetNearbyVacantLotsStream(fortBottom.GetGridPosition())
                .Where(block =>
                {
                    var highlandIsOneBlockAboveFortBottomLevel =
                        Math.Abs(block.GetGridPosition().y - (fortBottom.GetGridPosition().y + 1f)) < .5f;

                    return highlandIsOneBlockAboveFortBottomLevel && block.IsGrass() && block.IsVacant();
                });

            foreach (var highland in nearbyHighlands)
            {
                candidates.Add(
                    new Tuple<Block, Block>(fortBottom, highland)
                );
            }
        }
        
        var candidateCount = candidates.Count;
        
        if (candidateCount > 0)
        {
            var (fortBottom, highlands) = candidates.ElementAt(Random.Range(0, candidateCount));
           
            var target = highlands.transform.position;
            target.y += HeightOfABlock;

            var buildingSpawn = BlockFactory.Get().BuildingSpawn(fortBottom, target);
            fortBottom.Occupy(buildingSpawn);
            buildingSpawn.GetComponent<BuildingSpawn>().GroundHighlight(fortBottom);
            
            var buildingSpawnComponent = buildingSpawn.GetComponent<BuildingSpawn>();
            buildingSpawnComponent.CanStillConstruct = () => highlands != null;
            buildingSpawnComponent.CreateBuildingAction = () =>
            {
                var fortSpawn = Instantiate(fortSpawnTemplate);
                highlands.SetOccupantThatIsTailFromOtherBase(fortSpawn);

                return fortSpawn.gameObject;
            };
            
            _fortCount += 1;
        }
    }

    private bool CheckIfThereAreEnoughMountainToSpawnFort()
    {
        return _worldPlane.blocksRepository.StreamBlocks().Count(block => block.GetGridPosition().y > 1f) < 10;
    }
}