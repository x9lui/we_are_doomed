using UnityEngine;

/// <summary>
/// Before, dungeon generator used to generate dungeons in its Start() method,
/// that changed in order to gain more control, and now generation is done by
/// calling GenerateDungeon() method. The purpose of this script is to generate
/// a dungeon in the dungeon generator test scene inmediately the play button
/// is pressed.
/// </summary>
public class GenerateDungeonOnStart : MonoBehaviour
{
    [SerializeField] DungeonGenerator dungeonGenerator;
    void Start()
    {
        dungeonGenerator.GenerateDungeon();        
    }
}
