using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
public class LakeSpawn : MonoBehaviour
{
    
    private int _ticket = -1;
    private WorkQueue _workQueue;

    public BlockRelative blockRelative;
    private WorldPlane _worldPlane;

    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private void Awake()
    {
        blockRelative = GetComponent<BlockRelative>();
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
    }

    public void GroundHighlight(Block block)
    {
        var targetPosition = block.transform.position + Vector3.up * 0.1f;
        transform.position = targetPosition;
    }

    public void Update()
    {
        if (CanWorkThisFrame() && !LakeSpawner.CanSpawnLakeThere(blockRelative.block.GetGridPosition(), _worldPlane))
        {
            DestroyLakeSpawn();
        }
    }

    public void DestroyLakeSpawn()
    {
        blockRelative.block.DestroyOccupant();
    }
}
