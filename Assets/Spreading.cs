using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spreading : MonoBehaviour
{
    public GameObject sandBlockTemplate;
    public GameObject grassBlockTemplate;

    private WorldPlane _worldPlane;
    private Block _block;
    private SandSpreadController _sandSpreadController;

    private void Start()
    {
        _sandSpreadController = SandSpreadController.Get();
        _worldPlane = WorldPlane.Get();
        _block = GetComponentInChildren<Block>();
    }

    private void Update()
    {
        var sandSpreadController = _sandSpreadController;
        
        if (Random.value < sandSpreadController.chance)
        {
            var block = _worldPlane.GetNearbyBlocks(_block.GetGridPosition())
                .ToList()
                .OrderBy(_ => Random.value)
                .First();

            var score = _worldPlane.NatureScore(block.GetGridPosition(), sandSpreadController.natureScoreRadius);
            if (score < _sandSpreadController.spreadResistanceThreshold)
            {
                var sandBlockRoot = Instantiate(sandBlockTemplate);
                var sandBlock = sandBlockRoot.GetComponentInChildren<Block>();
                _worldPlane.ReplaceBlock(block, sandBlock);
            }
            else if (score > _sandSpreadController.spreadCombatThreshold)
            {
                var grassBlockRoot = Instantiate(grassBlockTemplate);
                var grassBlock = grassBlockRoot.GetComponentInChildren<Block>();
                _worldPlane.ReplaceBlock(_block, grassBlock);
            }
        }
    }
}