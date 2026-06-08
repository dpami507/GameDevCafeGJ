using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] GameObject dropPrefab;
    [SerializeField] UpgradeSO[] possibleUpgradeTypes;

    public void Drop()
    {
        GameObject dropGO = Instantiate(dropPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(-15, 15)));
        dropGO.GetComponent<UpgradePickup>().SetUpgrade(possibleUpgradeTypes[Random.Range(0, possibleUpgradeTypes.Length)]);
    }
}
