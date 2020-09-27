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

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _featureToggles = FeatureToggles.Get();
    }

    void Update()
    {
        if (!_featureToggles.fort) return;
        if (_fortCount > 1) return;

        if (Random.value < .001f)
        {
            if (_worldPlane.blocksRepository.StreamBlocks().Count(block => block.GetGridPosition().y > 1f) < 10) return;
            var candidates = _worldPlane.GetVacantBlocks()
                .Where(vacantBlock => vacantBlock.IsGroundLevel() &&
                                      _worldPlane.GetMajorityBlockTypeWithinRange(vacantBlock.GetGridPosition(), 1f)
                                      == Block.BlockType.Grass
                                      && _worldPlane.NoNearby(vacantBlock.GetGridPosition(), 2f,
                                          block => block.blockType == Block.BlockType.Water)
                )
                .SelectMany(fortBottom =>
                {
                    var nearbyHighlands = _worldPlane.GetNearbyVacantLots(fortBottom.GetGridPosition())
                        .Where(block =>
                        {
                            var highlandIsOneBlockAboveFortBottomLevel = Math.Abs(block.GetGridPosition().y - (fortBottom.GetGridPosition().y + 1f)) < .5f;
                            return highlandIsOneBlockAboveFortBottomLevel;
                        })
                        .ToList();

                    if (!nearbyHighlands.Any()) return new List<Tuple<Block, Block>>();

                    return nearbyHighlands
                        .Where(block => block.IsGrass() && block.IsVacant())
                        .Select(block => new Tuple<Block, Block>(fortBottom, block));
                })
                .ToList();

            var candidatesCount = candidates.Count;
            if (candidatesCount > 0)
            {
                var (fortBottom, highlands) = candidates[Random.Range(0, candidatesCount)];
                var fortSpawn = Instantiate(fortSpawnTemplate);
                fortBottom.Occupy(fortSpawn);
                highlands.SetOccupantThatIsTailFromOtherBase(fortSpawn);
                var target = highlands.transform.position;
                target.y = fortSpawn.transform.position.y;
                fortSpawn.transform.LookAt(target);

                _fortCount += 1;
            }
        }
    }
}