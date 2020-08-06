using UnityEngine;

public class FeatureToggles : MonoBehaviour
{
  public bool predators;
  public bool persons;

  public static FeatureToggles Get()
  {
    return FindObjectOfType<FeatureToggles>();
  }
}