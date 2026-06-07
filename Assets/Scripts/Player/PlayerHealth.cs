using UnityEngine;

public class PlayerHealth : BaseHealth
{
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        Events.TriggerCameraShake(1f);
    }
    protected override void Die()
    {
        base.Die();
    }
}
