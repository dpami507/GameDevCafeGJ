using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] float movementSpeed;
    [SerializeField] float stopDistance;
    [SerializeField] float maxDistance;

    [Header("Runaway")]
    [SerializeField] bool shouldRunaway;
    [SerializeField] float runAwayDistance;

    [Header("Attack")]
    [SerializeField] EnemyBaseAttack attack;

    Rigidbody2D rb;
    Transform target;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerMovement>()?.transform;
    }

    private void Update()
    {
        if(target == null)
        {
            target = FindFirstObjectByType<PlayerMovement>()?.transform;
            return;
        }

        float dist = Vector2.Distance(target.position, transform.position);
        
        Move(dist);
        attack.TryAttack(target);
    }
    void Move(float dist)
    {
        GameManager.instance.SetTileToColor(new Vector3Int((int)transform.position.x, (int)transform.position.y), Color.white);
        if (dist > stopDistance && dist < maxDistance)
        {
            Vector2 dir = target.position - transform.position;
            rb.linearVelocity = dir.normalized * movementSpeed;
        }
        else if(shouldRunaway && dist < runAwayDistance)
        {
            Vector2 dir = target.position - transform.position;
            rb.linearVelocity = -dir.normalized * movementSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
