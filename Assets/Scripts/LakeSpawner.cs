using System;
using System.Collections.Generic;
using System.Linq;
using blockInteractions;
using UnityEngine;

public class LakeSpawner : MonoBehaviour
{
    public GameObject lakeSpawnTemplate;
    private WorldPlane _worldPlane;
    private int _ticket = -1;
    private WorkQueue _workQueue;
    private static LakeSpawner _instance;

    private readonly List<Tuple<Vector3, float>> _candidates = new List<Tuple<Vector3, float>>();

    private const float SpawnTime = 1;

    private void Awake()
    {
        _instance = this;
    }

    public static LakeSpawner Get()
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
        if (_worldPlane.WorldIsEnding()) return;
        
        if (CanWorkThisFrame())
        {
            var candidates = _worldPlane.GetAllTopLots()
                .Where(pair => CanSpawnLakeThere(pair.Key))
                .Where(pair => !_candidates.Exists(candidate => candidate.Item1 == pair.Key))
                .Select(pair => new Tuple<Vector3, float>(pair.Key, Time.fixedTime + SpawnTime));

            _candidates.AddRange(candidates);

            if (_candidates.Count > 0)
            {
                var removeCandidates = new List<Tuple<Vector3, float>>();
                foreach (var candidate in _candidates)
                {
                    var timeToBringToLife = candidate.Item2;
                    if (Time.fixedTime > timeToBringToLife)
                    {
                        var blockPosition = candidate.Item1;
                        if (CanSpawnLakeThere(blockPosition))
                        {
                            if (_worldPlane.blocksRepository.HasAtPosition(blockPosition))
                            {
                                var block = _worldPlane.blocksRepository.GetAtPosition(blockPosition);
                                if (block.IsVacant())
                                {
                                    var lakeSpawn = Instantiate(lakeSpawnTemplate);
                                    block.Occupy(lakeSpawn);
                                    lakeSpawn.GetComponent<LakeSpawn>().GroundHighlight(block);
                                }
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

    private bool CanSpawnLakeThere(Vector3 point)
    {
        return CanSpawnLakeThere(point, _worldPlane);
    }

    public static bool CanSpawnLakeThere(Vector3 point, WorldPlane worldPlane)
    {
        return worldPlane.GetNearbyBlocks(point).All(block => block.GetHeight() > point.y && block.IsLot());
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
        
        var sound = BlockSoundLibrary.Get().GetSound(BlockSoundLibrary.BlockSound.Water);
        AudioSource.PlayClipAtPoint(sound, block.transform.position,  .02f * GameManager.MasterVolume);
    }
}