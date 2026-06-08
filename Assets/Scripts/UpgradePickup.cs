using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    [SerializeField] UpgradeSO upgradeType;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = upgradeType.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Events.IncreaseUpgrade(upgradeType.type, upgradeType.increaseAmount);
            Destroy(this.gameObject);
        }
    }
}
