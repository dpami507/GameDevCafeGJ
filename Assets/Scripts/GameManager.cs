using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dungenGenerationPrefab;
    DungeonGeneration generation;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] EnemySpawner spawner;

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
        CreateLevel();

        spawner.SpawnEnemies(generation.GetTilemap());
    }

    void CreateLevel()
    {
        // Create Level
        GameObject dungeonGenerationGO = Instantiate(dungenGenerationPrefab, this.transform);
        generation = dungeonGenerationGO.GetComponent<DungeonGeneration>();
        generation.Generate();

        // Create Player
        GameObject playerGO = Instantiate (playerPrefab, generation.GetStartPos(), Quaternion.identity);
    }
}
