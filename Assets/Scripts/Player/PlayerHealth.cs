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
    private void OnDestroy()
    {
        Events.TriggerPlayerHealth -= TakeDamage;
        Events.TriggerPlayerAddHealth -= AddHealth;
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

        Events.TriggerCameraShake(1f);
        Events.PlaySound("playerhit");
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
        Events.PlaySound("playerdie");
    }
}
