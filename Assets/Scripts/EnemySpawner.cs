using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] int enemiesToSpawn;
    int enemiesSpawned;
     
    public void SpawnEnemies(Tilemap tilemap)
    {
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
    void SpawnEnemy(Vector3Int pos)
    {
        int index = Random.Range(0, enemies.Length);
        Instantiate(enemies[index], pos, Quaternion.identity);
    }
}
