using UnityEngine;

public class EnemyBaseAttack : MonoBehaviour
{
    [Header("Attack Variables")]
    public float attackDistance;
    public int damage;
    public float attackCooldown;
    protected float lastAttacked;

    protected Transform target;

    private void Start()
    {
        UpdateStats();
    }

    public virtual void TryAttack(Transform target)
    {
        this.target = target;
        if (!CanAttack()) return;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackDistance, Vector2.up);
        foreach (var hit in hits)
        {
            if (hit.transform && hit.transform.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                Events.PlayerTakeDamage(damage);
                lastAttacked = 0;
            }
        }
    }
    public virtual void UpdateStats()
    {
        damage = Mathf.RoundToInt(damage * GameManager.instance.GetDificultyMultiplier());
        attackCooldown = attackCooldown / GameManager.instance.GetDificultyMultiplier();
    }
    protected bool CanAttack()
    {
        lastAttacked += Time.deltaTime;
        if (lastAttacked < attackCooldown) return false;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackDistance) return false;

        return true;
    }
}
