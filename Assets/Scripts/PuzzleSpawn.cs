using System.Linq;
using UnityEngine;

public abstract class PuzzleSpawn : MonoBehaviour
{
    private WorldPlane _worldPlane;
    private WorkQueue _workQueue;
    private int _ticket;

    public Vector3 spawnLookingTarget;
    public Vector3 spawnGridPosition;

    private bool _deactivated;
    private ConstructionMediator _constructionMediator;
    private Animator _animator;
    private static readonly int Highlight = Animator.StringToHash("Highlight");
    private bool _enabledHighlight;
    private static readonly int HighlightDisable = Animator.StringToHash("HighlightDisable");
    private static readonly int HighlightReady = Animator.StringToHash("HighlightReady");
    private static readonly int CanConstruct = Animator.StringToHash("CanConstruct");

    private void Awake()
    {
        BuildingSpawn.activeSpawns += 1;

        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
        _constructionMediator = ConstructionMediator.Get();
        _animator.SetTrigger(Highlight);
    }

    public abstract BuildingInfo GetBuildingInfo();

    public abstract Vector3 GetTarget();

    public abstract GameObject CreateBuildingAction();

    public abstract bool CanStillConstruct();

    public void DestroyBuildingSpawn()
    {
        Deactivate();

        Destroy(gameObject);
    }

    public void ActivateBuildingSpawn()
    {
        if (CanStillConstruct())
        {
            var target = GetTarget();
            if (target == Vector3.zero) target = spawnLookingTarget;
            PlaceHouse(target);

            _constructionMediator.BuildingCreated(GetBuildingInfo());

            DestroyBuildingSpawn();
        }
        else
        {
            PuzzleSpawnUtils.Get().PlayNotPossibleSound(spawnGridPosition);
        }
    }

    private void PlaceHouse(Vector3 lookingTarget)
    {
        var lot = GetSpawnBlock();
        var house = CreateBuildingAction();
        lot.Occupy(house);

        var target = lookingTarget;
        target.y = house.transform.position.y;
        house.transform.LookAt(target);
    }

    private void Deactivate()
    {
        if (!_deactivated)
        {
            _deactivated = true;
            BuildingSpawn.activeSpawns -= 1;
        }
    }

    void Update()
    {
        if (!CanWorkThisFrame()) return;

        var topBlock = _worldPlane.GetBlockAtTopOfStack(spawnGridPosition);
        if (!topBlock) return; // Probably just the end of the world or something!
        transform.position = topBlock.transform.position + Vector3.up * .1f;

        if (!_enabledHighlight)
        {
            if (CanStillConstruct())
            {
                _enabledHighlight = true;
                EnableReadyHighlight();
            }
        }
        else if (_enabledHighlight)
        {
            if (!CanStillConstruct())
            {
                _enabledHighlight = false;
                DisabledHighlight();
            }
        }
    }

    private void EnableReadyHighlight()
    {
        _animator.SetTrigger(HighlightReady);
        _animator.SetBool(CanConstruct, true);
    }

    private void DisabledHighlight()
    {
        _animator.SetTrigger(HighlightDisable);
        _animator.SetBool(CanConstruct, false);
    }

    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    protected Block GetSpawnBlock()
    {
        return _worldPlane.GetBlockAtTopOfStack(spawnGridPosition);
    }
}