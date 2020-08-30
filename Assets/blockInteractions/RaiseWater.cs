using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace blockInteractions
{
    public class RaiseWater : MonoBehaviour
    {
        public GameObject waterBlockTemplate;
        private static RaiseWater _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static RaiseWater Get()
        {
            return _instance;
        }

        public void Use(Block block)
        {
            var water = Instantiate(waterBlockTemplate);
            var waterBlock = water.GetComponentInChildren<Block>();
            WorldPlane.Get().AddBlockOnTopOf(waterBlock, water, block);
            waterBlock.ShortFreeze();
        }   
    }
}