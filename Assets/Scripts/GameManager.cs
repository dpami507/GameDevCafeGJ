using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dungenGenerationPrefab;
    DungeonGeneration generation;
    [SerializeField] GameObject playerPrefab;
    GameObject player;

    [SerializeField] EnemySpawner spawner;

    [Header("Gameplay Stats")]
    [SerializeField] int currentFloor;

    [Header("Gameplay UI")]
    [SerializeField] TMP_Text timer;
    [SerializeField] TMP_Text level;
    [SerializeField] TMP_Text difficulty;


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
        currentFloor = 0;
        CreateLevel();
    }
    public float GetDificultyMultiplier() => 1 + Mathf.Pow(currentFloor, 2) / 10f - (1 / 10f);
    private void Update()
    {
        UpdateUI();
    }
    void UpdateUI()
    {
        timer.text = GetPlayTime();
        level.text = "Lvl: " + currentFloor.ToString();
        difficulty.text = "x" + GetDificultyMultiplier().ToString("0.0");
    }
    string GetPlayTime()
    {
        float time = Time.time;
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        return $"{minutes.ToString("00")}:{seconds.ToString("00")}";
    }
    public void CreateLevel()
    {
        currentFloor++;

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
