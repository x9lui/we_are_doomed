using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PickUpGOAndProbability
{
    public GameObject pickUpGO;
    public float probability;
}

public class SinglePlayerGameManager : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;

    //probabilities are not normalized
    [SerializeField] private List<PickUpGOAndProbability> pickupsAndProbabilities;
    float totalPickupProbability = 0;
    List<DungeonGenerator.Cell> rooms;
    DungeonGenerator.Cell spawnRoom;
    DungeonGenerator.Cell finalRoom;

    [SerializeField] GameObject player;
    [SerializeField] GameObject imp;

    void Awake()
    {
        //Init game after dungeon is generated
        dungeonGenerator.DungeonGenerated += InitGame;

        foreach (PickUpGOAndProbability el in pickupsAndProbabilities)
        {
            totalPickupProbability += el.probability;
        }
    }
    void Start()
    {
        dungeonGenerator.GenerateDungeon();
    }

    void InitGame()
    {
        rooms = dungeonGenerator.GetDungeonRooms();

        //Select a trivial room as the spawn
        spawnRoom = rooms[Random.Range(0, rooms.Count - 1)];

        SelectFinalRoom();

        //Spawn player
        Instantiate(player, GetRandomPositionInsideCell3D(spawnRoom) + Vector3.up * 2f, Quaternion.identity);

        //Generate creatures in all rooms 
        foreach (DungeonGenerator.Cell room in rooms)
        {
            for (int i = 0; i < 3; i++)
                Instantiate(imp, GetRandomPositionInsideCell3D(room) + Vector3.up * 2f, Quaternion.identity);
        }

        //Generate pick ups
        foreach (DungeonGenerator.Cell room in rooms)
        {
            Instantiate(GetRandomPickup(), GetCenterCell3D(room) + Vector3.up * 1f, Quaternion.identity);
        }
    }

    /// <summary>
    /// Select the farthest room from the spawn as the final room.
    /// A spawn room must have been chosen before calling this method.
    /// </summary>
    private void SelectFinalRoom()
    {
        float finalRoomDistanceFromSpawnRoom = float.MinValue;
        foreach (DungeonGenerator.Cell room in rooms)
        {
            if (Vector2.Distance(spawnRoom.position, room.position) > finalRoomDistanceFromSpawnRoom)
            {
                finalRoomDistanceFromSpawnRoom = Vector2.Distance(spawnRoom.position, room.position);
                finalRoom = room;
            }
        }
    }


    GameObject GetRandomPickup()
    {
        float acumulatedProbability = Random.Range(0, totalPickupProbability);
        foreach (PickUpGOAndProbability el in pickupsAndProbabilities)
        {
            acumulatedProbability -= el.probability;
            if (acumulatedProbability <= 0) return el.pickUpGO;
        }
        throw new System.Exception("Error getting a random pick up");
    }

    /// <summary>
    /// Helper function used to return Vector3 positions inside cells instead
    /// of Vector2.
    /// </summary>
    /// <param name="cell">
    /// The cell which the random point will be contained.
    /// </param>
    /// <returns>
    /// A random position inside a cell
    /// </returns>
    Vector3 GetRandomPositionInsideCell3D(DungeonGenerator.Cell cell)
    {
        Vector2 position2d = cell.GetRandomPositionInside(1.2f);
        return new Vector3(position2d.x, 0f, position2d.y);
    }

    /// <summary>
    /// Helper function used to return Vector3 positions inside cells instead
    /// of Vector2.
    /// </summary>
    /// <param name="cell">
    /// The cell which the center position is going to be returned.
    /// </param>
    /// <returns>
    /// The center position of the room
    /// </returns>
    Vector3 GetCenterCell3D(DungeonGenerator.Cell cell)
    {
        Vector2 position2d = cell.GetCenter();
        return new Vector3(position2d.x, 0f, position2d.y);
    }
}
