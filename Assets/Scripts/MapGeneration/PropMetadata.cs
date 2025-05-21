using UnityEngine;

public enum PropPlacementType
{
    NearWall,
    Center,
    Grid,
    Scatter,
    Hanging
}

[CreateAssetMenu(menuName = "Dungeon/Prop Metadata")]
public class PropMetadata : ScriptableObject
{
    public PropPlacementType placementType;
    public float spawnChance = 1f;
    public float minDistanceBetween = 1.5f;
    public bool isCeilingObject = false;
}
