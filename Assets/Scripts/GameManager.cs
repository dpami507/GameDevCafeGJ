using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;

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
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;

    public bool gameRunning;
    public bool gameOver;

    public static GameManager instance;

    InputAction pauseAction;
    public DungeonGeneration GetDungeon() => generation;
    public void SetTileToColor(Vector3Int pos, Color color) => generation.GetTilemap().SetColor(pos, color);

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
    private void OnDestroy()
    {
        Events.TriggerGenerateNewLevel -= CreateLevel;

        instance = null;
        if (spawner != null && spawner.gameObject != null)
            Destroy(spawner.gameObject);

        if (generation != null && generation.gameObject != null)
            Destroy(generation.gameObject);

        if (player != null)
            Destroy(player);
    }

    void Start()
    {
        StartGame();
        gameRunning = true;
        gameOver = false;

        Events.TriggerGenerateNewLevel -= CreateLevel;
        Events.TriggerGenerateNewLevel += CreateLevel;

        pauseAction = InputSystem.actions.FindAction("Pause");
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

        if (pauseAction.IsPressed())
        {
            Pause();
        }
    }
    void UpdateUI()
    {
        timer.text = GetPlayTime();
        level.text = "Lvl: " + currentFloor.ToString();
        difficulty.text = "x" + GetDificultyMultiplier().ToString("0.0");
    }
    public void Pause()
    {
        gameRunning = false;
        pauseMenu.SetActive(true);
    }
    public void Unpause()
    {
        gameRunning = true;
        pauseMenu.SetActive(false);
    }
    public void GameOver()
    {
        gameRunning = false;
        gameOverMenu.SetActive(true);
        gameOver = true;
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

        // Create Level
        if(generation == null)
        {
            GameObject dungeonGenerationGO = Instantiate(dungenGenerationPrefab, instance.transform);
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
        if (spawner != null)
            spawner.SpawnEnemies(generation.GetTilemap());

        // Clear Enemies near player
        if (spawner != null && player != null)
            spawner.ClearEntitiesInRadius(player.transform.position, 5);

        // Create Boss
        if (spawner != null)
            spawner.SpawnBoss(generation.GetEndPos());
    }
}
