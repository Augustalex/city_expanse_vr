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