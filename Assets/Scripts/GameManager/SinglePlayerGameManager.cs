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

    //Loading variables
    float totalLoading = 0f;

    //Loading percentages variables
    float dungeonGenerationLoadingPercentage = 0f;

    //nextLevelItem is the item the player interacts with to pass the level.
    [SerializeField] private GameObject nextLevelItem;

    //Gameobject parents/containers
    private GameObject enemyParent;

    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;

    [Header("Enemies")]
    [SerializeField] GameObject imp;
    [SerializeField] GameObject zombieSoldier;
    [SerializeField] GameObject cacodemon;

    //Events
    public event System.Action LevelStart;
    public event System.Action<float> LevelLoadPercentage;
    public event System.Action LevelFinish;

    void Awake()
    {
        //Singleton 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //We need to calculate the total probability because it isn't normalized
        totalPickupProbability = 0;
        foreach (PickUpGOAndProbability el in pickupsAndProbabilities)
        {
            totalPickupProbability += el.probability;
        }
    }

    void Start()
    {
        //Init game after dungeon is generated
        dungeonGenerator.DungeonGenerated += StartLevel;

        dungeonGenerator.DungeonPercentageGenerated += UpdateLoadingPercentageOfDungeonGenerator;

        dungeonGenerator.GenerateDungeon();

        //Generate player and disable it until game is ready
        playerInstance = Instantiate(playerPrefab);
        playerInstance.SetActive(false);

    }

    //For testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) NextLevel();    
    }

    /// <summary>
    /// The main purpose of this function is to be suscribed to the event
    /// of DungeonGenerator that is invoked when it updates it loading process
    /// value.
    /// </summary>
    /// <param name="percentage"></param>
    private void UpdateLoadingPercentageOfDungeonGenerator(float percentage)
    {
        dungeonGenerationLoadingPercentage = percentage;
        UpdateLevelLoadingPercentage();
    }

    private void UpdateLevelLoadingPercentage()
    {
        totalLoading = 0f;

        /*In our case we only have a part, and the 100% (1f) of the loading process
        has been associated with that part
        Example with more processes:
        totalLoading += process1 * 0.3f
        totalLoading += process2 * 0.2f
        totalLoading += process3 * 0.5f
        */
        totalLoading += dungeonGenerationLoadingPercentage * 1f;

        LevelLoadPercentage?.Invoke(totalLoading);
    }
    
    void StartLevel()
    {
        LevelStart?.Invoke();

        rooms = dungeonGenerator.GetDungeonRooms();

        //Select a trivial room as the spawn
        spawnRoom = rooms[Random.Range(0, rooms.Count - 1)];

        ConfigureFinalRoom();

        //Move player to the spawn room and enable it
        playerInstance.transform.position = GetRandomPositionInsideCell3D(spawnRoom) + Vector3.up * 2f;
        playerInstance.SetActive(true);

        GenerateCreatures();
        GeneratePickUps();
    }
    /// <summary>
    /// Selects the final room and generates the nextLevelItem in its center
    /// </summary>
    private void ConfigureFinalRoom()
    {
        SelectFinalRoom();
        Instantiate(nextLevelItem, GetCellCenter3D(finalRoom) + Vector3.up * 1, Quaternion.identity);
    }

    /// <summary>
    /// Generate creatures in all the rooms except the spawn
    /// </summary>
    private void GenerateCreatures()
    {
        enemyParent = new GameObject("EnemyParent");
        foreach (DungeonGenerator.Cell room in rooms)
        {
            if (room == spawnRoom) continue;
            for (int i = 0; i < Random.Range(currentLevel / 2 + 1, currentLevel + 1); i++)
                Instantiate(GetRandomEnemy(), GetRandomPositionInsideCell3D(room) + Vector3.up * 2f, Quaternion.identity, enemyParent.transform);
        }
    }

    GameObject GetRandomEnemy()
    {
        float val = Random.value;
        if (val <= 0.33f) return imp;
        if (val <= 0.66f) return zombieSoldier;
        return cacodemon;
    }

    private void DestroyCreatures()
    {
        Destroy(enemyParent);
    }

    private void DestroyDungeon()
    {
        dungeonGenerator.DestroyDungeon();
    }

    private void GeneratePickUps()
    {
        foreach (DungeonGenerator.Cell room in rooms)
        {
            if (room == spawnRoom) continue;
            if (room == finalRoom) continue;
            if (Random.value < 0.1f) continue;
            Instantiate(GetRandomPickup(), GetCellCenter3D(room) + Vector3.up * 1f, Quaternion.identity);
        }
    }

    public void NextLevel()
    {
        LevelFinish?.Invoke();

        playerInstance.SetActive(false);

        currentLevel++;

        //Destroy all the enemies
        Destroy(enemyParent);
        //Destroy all the dungeon and create a new parent
        DestroyDungeon();
        DestroyCreatures();

        //Save player state

        //Reconfigure probabilities

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