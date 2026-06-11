using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] GameObject[] enemies;
    [SerializeField] int enemiesToSpawn;
    int enemiesSpawned;
    int originalEnemiesToSpawn;

    [Header("Bosses")]
    [SerializeField] GameObject[] bosses;

    public List<GameObject> spawnedEntities = new List<GameObject>();

    private void Awake()
    {
        originalEnemiesToSpawn = enemiesToSpawn;
    }
    public void SpawnEnemies(Tilemap tilemap)
    {
        enemiesSpawned = 0;
        enemiesToSpawn = Mathf.RoundToInt(originalEnemiesToSpawn * (((GameManager.instance.GetDificultyMultiplier() - 1) / 2.0f) + 1));
        Debug.Log($"Spawning Enemies {enemiesToSpawn}, {originalEnemiesToSpawn} * {GameManager.instance.GetDificultyMultiplier()}");

        int minX = tilemap.cellBounds.xMin;
        int minY = tilemap.cellBounds.yMin;
        int maxX = tilemap.cellBounds.xMax;
        int maxY = tilemap.cellBounds.yMax;

        while (enemiesSpawned < enemiesToSpawn)
        {
            Vector3Int possibleSpawnPos = new Vector3Int(Random.Range(minX, maxX), Random.Range(minY, maxY));

            if (!tilemap.HasTile(possibleSpawnPos)) continue;

            SpawnEnemy(possibleSpawnPos);

            enemiesSpawned++;
        }
    }
    GameObject SpawnEnemy(Vector3Int pos)
    {
        int index = Random.Range(0, enemies.Length);
        GameObject spawnedEnemy = Instantiate(enemies[index], pos, Quaternion.identity);

        spawnedEntities.Add(spawnedEnemy);

        return spawnedEnemy;
    }

    public GameObject SpawnBoss(Vector3 pos)
    {
        int index = Random.Range(0, bosses.Length);
        GameObject spawnedBoss = Instantiate(bosses[index], pos, Quaternion.identity);

        spawnedEntities.Add(spawnedBoss);

        return spawnedBoss;
    }
    public void ClearEntities()
    {
        foreach(var entity in spawnedEntities)
        {
            Destroy(entity);
        }
        spawnedEntities.Clear();
    }
    public void ClearEntitiesInRadius(Vector2 center, float radius)
    {
        foreach (var entity in spawnedEntities)
        {
            if(Vector2.Distance(center, entity.transform.position) <= radius)
            {
                Destroy(entity);
            }
        }
    }
}
