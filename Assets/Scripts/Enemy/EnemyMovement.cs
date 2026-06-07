using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] float movementSpeed;
    [SerializeField] float stopDistance;
    [SerializeField] float maxDistance;

    [Header("Attack Variables")]
    [SerializeField] float attackDistance;
    [SerializeField] int damage;
    [SerializeField] float attackCooldown;
    float lastAttacked;

    Rigidbody2D rb;
    Transform target;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        float dist = Vector2.Distance(target.position, transform.position);
        
        Move(dist);
        TryAttack(dist);
    }
    void Move(float dist)
    {
        if (dist > stopDistance && dist < maxDistance)
        {
            Vector2 dir = target.position - transform.position;
            rb.linearVelocity = dir.normalized * movementSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    void TryAttack(float dist)
    {
        lastAttacked += Time.deltaTime;
        if (lastAttacked < attackCooldown) return;

        if(dist <= attackDistance)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackDistance, Vector2.up);
            foreach(var hit in hits)
            {
                if(hit.transform && hit.transform.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    lastAttacked = 0;
                }
            }
        }
    }
}
