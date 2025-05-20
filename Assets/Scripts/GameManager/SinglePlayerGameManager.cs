using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PickUpGOAndProbability
{
    public GameObject pickUpGO;
    public float probability;
}

//Singleton class
public class SinglePlayerGameManager : MonoBehaviour
{
    public static SinglePlayerGameManager Instance { get; private set; }
    [SerializeField] private DungeonGenerator dungeonGenerator;

    //probabilities are not normalized
    [SerializeField] private List<PickUpGOAndProbability> pickupsAndProbabilities;
    float totalPickupProbability = 0;
    List<DungeonGenerator.Cell> rooms;
    DungeonGenerator.Cell spawnRoom;
    DungeonGenerator.Cell finalRoom;

    int currentLevel = 1;

    //nextLevelItem is the item the player interacts with to pass the level.
    [SerializeField] private GameObject nextLevelItem;

    //Gameobject parents/containers
    private GameObject dungeonParent;
    private GameObject enemyParent;

    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;

    [Header("Enemies")]
    [SerializeField] GameObject imp;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //Init game after dungeon is generated
        dungeonGenerator.DungeonGenerated += StartLevel;

        foreach (PickUpGOAndProbability el in pickupsAndProbabilities)
        {
            totalPickupProbability += el.probability;
        }
    }

    void Start()
    {
        dungeonParent = new GameObject("DugeonParent");
        dungeonGenerator.dungeonParent = dungeonParent.transform;
        dungeonGenerator.GenerateDungeon();
        playerInstance = Instantiate(playerPrefab);
        playerInstance.SetActive(false);
    }

    void StartLevel()
    {
        enemyParent = new GameObject("EnemyParent");

        rooms = dungeonGenerator.GetDungeonRooms();

        //Select a trivial room as the spawn
        spawnRoom = rooms[Random.Range(0, rooms.Count - 1)];

        //Select final room and generate the nextLevelItem in its center
        SelectFinalRoom();
        Instantiate(nextLevelItem, GetCellCenter3D(finalRoom) + Vector3.up * 1, Quaternion.identity);

        //Move player to the spawn room and enable it
        playerInstance.transform.position = GetRandomPositionInsideCell3D(spawnRoom) + Vector3.up * 2f;
        playerInstance.SetActive(true);

        //Generate creatures in all rooms except the spawn 
        foreach (DungeonGenerator.Cell room in rooms)
        {
            if (room == spawnRoom) continue;
            for (int i = 0; i < Random.Range(currentLevel / 2 + 1, currentLevel + 1); i++)
                Instantiate(imp, GetRandomPositionInsideCell3D(room) + Vector3.up * 2f, Quaternion.identity, enemyParent.transform);
        }

        //Generate pick ups
        foreach (DungeonGenerator.Cell room in rooms)
        {
            Instantiate(GetRandomPickup(), GetCellCenter3D(room) + Vector3.up * 1f, Quaternion.identity);
        }
    }

    public void NextLevel()
    {
        playerInstance.SetActive(false);

        currentLevel++;

        //Destroy all the enemies
        Destroy(enemyParent);

        //Destroy all the dungeon and create a new parent
        Destroy(dungeonGenerator.dungeonParent.gameObject);
        GameObject dungeonRoot = new GameObject("DugeonRoot");
        dungeonGenerator.dungeonParent = dungeonRoot.transform;

        //Save player state

        //Reconfigure probabilities


        //display loading scene

        //Change number of cells and regenerate dungeon
        dungeonGenerator.numberOfCells = (int)(dungeonGenerator.numberOfCells * 1.2f);
        dungeonGenerator.GenerateDungeon();
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
    Vector3 GetCellCenter3D(DungeonGenerator.Cell cell)
    {
        Vector2 position2d = cell.GetCenter();
        return new Vector3(position2d.x, 0f, position2d.y);
    }
}