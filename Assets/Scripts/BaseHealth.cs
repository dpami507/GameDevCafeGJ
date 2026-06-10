using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Update()
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
    }
    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }
    public float GetHealthAsPercentage() => currentHealth / maxHealth;
}
