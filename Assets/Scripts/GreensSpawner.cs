using System;
using System.Collections.Generic;
using System.Linq;
using blockInteractions;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BlockRelative))]
public class GreensSpawner : MonoBehaviour
{
    public GameObject greensSpawn;
    private WorldPlane _worldPlane;
    private int _ticket = -1;
    private WorkQueue _workQueue;
    private static GreensSpawner _instance;

    private readonly List<Tuple<Vector3, float>> _candidates = new List<Tuple<Vector3, float>>();

    private const float SpawnTime = 2;

    private void Awake()
    {
        _instance = this;
    }

    public static GreensSpawner Get()
    {
        return _instance;
    }

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
    }

    void Update()
    {
        if (!_worldPlane.WorldGenerationDone()) return;

        if (CanWorkThisFrame())
        {
            if (Random.value < .1f)
            {
                var candidates = _worldPlane.GetAllTopLots()
                    .OrderBy(_ => Random.value)
                    .Take(1)
                    .Where(pair => CanSpawnGreensThere(pair.Key))
                    .Where(pair => !_candidates.Exists(candidate => candidate.Item1 == pair.Key))
                    .Select(pair => new Tuple<Vector3, float>(pair.Key, Time.fixedTime + SpawnTime));
                _candidates.AddRange(candidates);
            }
           

            if (_candidates.Count > 0)
            {
                var removeCandidates = new List<Tuple<Vector3, float>>();
                foreach (var candidate in _candidates)
                {
                    var timeToBringToLife = candidate.Item2;
                    if (Time.fixedTime > timeToBringToLife)
                    {
                        var blockPosition = candidate.Item1;
                        if (_worldPlane.blocksRepository.HasAtPosition(blockPosition) &&
                            CanSpawnGreensThere(blockPosition))
                        {
                            var block = _worldPlane.blocksRepository.GetAtPosition(blockPosition);
                            if (block.IsVacant())
                            {
                                var lakeSpawn = Instantiate(greensSpawn);
                                block.Occupy(lakeSpawn);
                            }
                        }

                        removeCandidates.Add(candidate);
                    }
                }

                foreach (var removeCandidate in removeCandidates)
                {
                    _candidates.Remove(removeCandidate);
                }
            }
        }
    }

    private bool CanSpawnGreensThere(Vector3 point)
    {
        return CanSpawnGreensThere(point, _worldPlane);
    }

    public static bool CanSpawnGreensThere(Vector3 point, WorldPlane worldPlane)
    {
        var block = worldPlane.blocksRepository.GetAtPosition(point);
        return block.IsGrass() && block.IsVacant() && block.BelowCloudLevel();
    }

    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    public void ActivateLakeSpawn(LakeSpawn lakeSpawn)
    {
        var block = lakeSpawn.blockRelative.block;
        block.DestroyOccupant();
        RaiseWater.Get().Use(block);
    }
}