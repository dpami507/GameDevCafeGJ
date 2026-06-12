using UnityEngine;

public class PlayerHealth : BaseHealth
{
    float originalMaxHealth;
    private void Start()
    {
        Events.TriggerPlayerHealth += TakeDamage;
        Events.TriggerPlayerAddHealth += AddHealth;

        SetupHealth();
        originalMaxHealth = maxHealth;
    }
    public override void Update()
    {
        maxHealth = (int)(originalMaxHealth * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.MaxHealth));
        if(currentHealth <= 0)
        {
            Die();
        }    
    }
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log($"Player Took Damage! -{amount}, {currentHealth}/{maxHealth}");

        Events.TriggerCameraShake(1f);
        ApplicationManager.instance.PlaySound("playerhit");
    }
    public override void AddHealth(float amount)
    {
        base.AddHealth(amount);
    }
    protected override void Die()
    {
        //base.Die();
        if (GameManager.instance.gameOver) return;
        Debug.Log("Player Died");
        GameManager.instance.GameOver();
        ApplicationManager.instance.PlaySound("playerdie");
    }
}
