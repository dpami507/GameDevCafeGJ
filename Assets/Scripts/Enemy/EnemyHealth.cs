using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyHealth : BaseHealth
{
    [Header("Visual")]
    [SerializeField] GameObject damageIndicatorPrefab;
    [SerializeField] float dmgIndicatorDistance;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float vanishTime;

    Color originalColor;

    private void Start()
    {
        originalColor = spriteRenderer.color;
        SetupHealth();
    }
    public override void SetupHealth()
    {
        maxHealth = Mathf.RoundToInt(maxHealth * GameManager.instance.GetDificultyMultiplier());
        currentHealth = maxHealth;
    }

    public override void TakeDamage(float amount)
    {
        SpawnDamageIndicator(amount);
        StartCoroutine(nameof(HitFeedback));

        base.TakeDamage(amount);
    }
    protected override void Die()
    {
        base.Die();
        GameManager.instance.GetDungeon().SetTilesToColor(transform.position, 5, Color.HSVToRGB((Random.value), 1f, 1f));
        if(TryGetComponent<EnemyDrop>(out EnemyDrop enemyDrop))
        {
            enemyDrop.Drop();
        }
    }
    void SpawnDamageIndicator(float amount)
    {
        float angle = Random.Range(0, 360);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * dmgIndicatorDistance;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * dmgIndicatorDistance;
        Vector2 dmgIndicatorPos = new Vector2(x + transform.position.x, y + transform.position.y);
        GameObject dmgIndcGO = Instantiate(damageIndicatorPrefab, dmgIndicatorPos, Quaternion.identity);
        TMP_Text dmgTxt = dmgIndcGO.transform.GetComponentInChildren<TMP_Text>();
        dmgTxt.text = amount.ToString();

        Destroy(dmgIndcGO, 1f);
    }
    IEnumerator HitFeedback()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(vanishTime);
        spriteRenderer.color = originalColor;
    }
}
