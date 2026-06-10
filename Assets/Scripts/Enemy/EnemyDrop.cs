using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] GameObject dropPrefab;
    [SerializeField] Drop[] possibleDrops;
    [SerializeField] float dropDist;

    public void Drop()
    {
        foreach(var drop in possibleDrops)
        {
            GameObject dropGO = Instantiate(dropPrefab, GetRandomDrop(), Quaternion.Euler(0, 0, Random.Range(-15, 15)));

            int check = Random.Range(0, 100);
            if(drop.chance >= check)
            {
                int index = Random.Range(0, drop.drops.Length);
                dropGO.GetComponent<UpgradePickup>().SetUpgrade(drop.drops[index]);
            }
        }
    }
    Vector3 GetRandomDrop()
    {
        float rad = Random.Range(0, 2 * Mathf.PI);

        float x = Mathf.Cos(rad) * dropDist;
        float y = Mathf.Sin(rad) * dropDist;

        Vector2 pos = new Vector2(x + transform.position.x, y + transform.position.y);
        return pos;
    }
}
[System.Serializable]
public class Drop
{
    public int chance;
    public UpgradeSO[] drops;
}