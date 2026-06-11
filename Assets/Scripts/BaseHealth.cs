using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int maxHealth;

    private void Start()
    {
        SetupHealth();
    }
    public virtual void SetupHealth()
    {
        currentHealth = maxHealth;
    }
    public virtual void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }
    public virtual void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }
    public float GetHealthAsPercentage() => currentHealth / maxHealth;
}
