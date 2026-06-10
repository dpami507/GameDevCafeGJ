using UnityEngine;

public class PlayerHealth : BaseHealth
{
    private void Start()
    {
        Events.TriggerPlayerHealth += TakeDamage;
        Events.TriggerPlayerAddHealth += AddHealth;

        SetupHealth();
    }
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log($"Player Took Damage! -{amount}, {currentHealth}/{maxHealth}");

        Events.TriggerCameraShake(1f);
    }
    public override void AddHealth(float amount)
    {
        base.AddHealth(amount);
    }
    protected override void Die()
    {
        //base.Die();
        Debug.Log("Player Died");
    }
}
