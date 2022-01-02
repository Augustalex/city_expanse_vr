using System;

public class ConstructionMediator
{
    private static ConstructionMediator _instance;
    private static bool _instanceSet;

    public static ConstructionMediator Get()
    {
        if (!_instanceSet)
        {
            _instanceSet = true;
            _instance = new ConstructionMediator();
        }

        return _instance;
    }
    
    public event Action<BuildingInfo> OnBuildingCreated;

    public void BuildingCreated(BuildingInfo info)
    {
        OnBuildingCreated?.Invoke(info);
    }
}

public struct BuildingInfo
{
    public int devotees;
}