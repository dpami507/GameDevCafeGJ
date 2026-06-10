using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dungenGenerationPrefab;
    DungeonGeneration generation;
    [SerializeField] GameObject playerPrefab;
    GameObject player;

    [SerializeField] EnemySpawner spawner;

    [Header("Gameplay Stats")]
    [SerializeField] int currentFloor;

    public static GameManager instance;

    public DungeonGeneration GetDungeon() => generation;
    public void SetTileToColor(Vector3Int pos, Color color) => generation.GetTilemap().SetColor(pos, color);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        StartGame();

        Events.TriggerGenerateNewLevel += CreateLevel;
    }
    void StartGame()
    {
        currentFloor = 10;
        CreateLevel();
    }
    public float GetDificultyMultiplier() => 1 + Mathf.Pow(currentFloor, 2) / 10f;
    public void CreateLevel()
    {
        // clear all enemies
        spawner.ClearEntities();

        // Create Level
        if(generation == null)
        {
            GameObject dungeonGenerationGO = Instantiate(dungenGenerationPrefab, this.transform);
            generation = dungeonGenerationGO.GetComponent<DungeonGeneration>();
        }
        generation.Generate();

        // Create Player
        if (player == null)
        {
            player = Instantiate(playerPrefab, generation.GetStartPos(), Quaternion.identity);
        }
        else
        {
            player.transform.position = generation.GetStartPos();
        }

        // Spawn Enemies
        spawner.SpawnEnemies(generation.GetTilemap());

        // Clear Enemies near player
        spawner.ClearEntitiesInRadius(player.transform.position, 5);

        // Create Boss
        GameObject bossGO = spawner.SpawnBoss(generation.GetEndPos());
    }
}
