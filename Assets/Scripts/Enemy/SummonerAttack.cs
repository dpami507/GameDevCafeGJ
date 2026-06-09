using System.Collections;
using UnityEngine;

public class SummonerAttack : EnemyBaseAttack
{
    [Header("Summoner")]
    [SerializeField] GameObject spikePrefab;
    [SerializeField] float spikeSpeed;
    [SerializeField] float angleVariance;
    [SerializeField] int spikeCount;
    [SerializeField] float spawnDistance;
    [SerializeField] float spikeLaunchGap;

    public override void TryAttack(Transform target)
    {
        this.target = target;
        if (!CanAttack()) return;

        StartCoroutine(nameof(CreateSpikes));

        lastAttacked = 0;
    }
    IEnumerator CreateSpikes()
    {
        for (int i = 0; i < spikeCount; i++) {
            yield return new WaitForSeconds(spikeLaunchGap);
            CreateSpike(target);
        }
    }
    void CreateSpike(Transform target)
    {
        Vector2 pos = GetPosAroundEnemy();

        // Calculate angle with varience
        Vector2 dir = (Vector2)target.position - pos;
        float anlgeToPlayer = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float randSpread = Random.Range(-angleVariance, angleVariance);
        Quaternion spikeDir = Quaternion.Euler(0, 0, anlgeToPlayer + randSpread - 90);

        GameObject createdSpikeGO = Instantiate(spikePrefab, pos, spikeDir);
        Spike spike = createdSpikeGO.GetComponent<Spike>();
        spike.StartSpike(spikeSpeed, damage);
    }
    Vector2 GetPosAroundEnemy()
    {
        float rad = Random.Range(0, 2 * Mathf.PI);

        float x = Mathf.Cos(rad) * spawnDistance;
        float y = Mathf.Sin(rad) * spawnDistance;

        Vector2 pos = new Vector2(x + transform.position.x, y + transform.position.y);
        return pos;
    }
}
