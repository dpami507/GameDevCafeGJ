using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] float movementSpeed;
    [SerializeField] float stopDistance;

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
        if(dist > stopDistance)
        {
            Vector2 dir = target.position - transform.position;
            rb.linearVelocity = dir.normalized * movementSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
