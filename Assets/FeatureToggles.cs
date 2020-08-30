using System;
using UnityEngine;

public class FeatureToggles : MonoBehaviour
{
  public bool predators;
  public bool persons;
  public bool farmsGrowAtRandom;
  public bool biomes;

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