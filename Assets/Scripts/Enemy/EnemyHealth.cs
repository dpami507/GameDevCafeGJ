using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [Header("Visual")]
    [SerializeField] GameObject damageIndicatorPrefab;
    [SerializeField] float dmgIndicatorDistance;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float vanishTime;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if(currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeDamage(float amount)
    {
        SpawnDamageIndicator(amount);
        currentHealth -= Mathf.RoundToInt(amount);

        StartCoroutine(nameof(HitFeedback));
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    void SpawnDamageIndicator(float amount)
    {
        float angle = Random.Range(0, 360);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) + transform.position.x;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) + transform.position.y;
        Vector2 dmgIndicatorPos = new Vector2(x, y) * dmgIndicatorDistance;
        GameObject dmgIndcGO = Instantiate(damageIndicatorPrefab, dmgIndicatorPos, Quaternion.identity);
        TMP_Text dmgTxt = dmgIndcGO.transform.GetComponentInChildren<TMP_Text>();
        dmgTxt.text = amount.ToString();

        Destroy(dmgIndcGO, 1f);
    }
    IEnumerator HitFeedback()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(vanishTime);
        spriteRenderer.color = originalColor;
    }
}
