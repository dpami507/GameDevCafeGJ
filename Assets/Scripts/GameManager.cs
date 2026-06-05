using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dungenGenerationPrefab;
    DungeonGeneration generation;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] EnemySpawner spawner;

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
