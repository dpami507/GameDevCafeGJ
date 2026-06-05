using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dungenGenerationPrefab;
    DungeonGeneration generation;
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        CreateLevel();
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
