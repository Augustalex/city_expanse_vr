using System;
using UnityEngine;

public class FeatureToggles : MonoBehaviour
{
  public bool predators;
  public bool persons;
  public bool farmsGrowAtRandom;
  public bool biomes;
  public bool fort;
  public bool woodcutters;
  public bool digAnywhere;
  public bool storms;
  public bool desertsAreBeaches;
  public bool houseSpawn;
  public bool docksSpawn;
  public bool farmExpandOnItsOwn;
  public bool interactionOverlay;
  public bool interactionGhost;

  private static FeatureToggles _featureToggles;

  private void Awake()
  {
    _featureToggles = this;
  }

  public static FeatureToggles Get()
  {
    return _featureToggles;
  }
}