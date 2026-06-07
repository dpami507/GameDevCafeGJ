using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int maxHealth;

    private void Start()
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
        currentHealth -= Mathf.RoundToInt(amount);
    }

    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
