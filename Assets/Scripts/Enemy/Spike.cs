using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Spike : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] spikeSprites;

    Rigidbody2D rb;
    float damage;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spikeSprites[Random.Range(0, spikeSprites.Length)];

        rb = GetComponent<Rigidbody2D>();
    }
    public void StartSpike(float velocity, float damage)
    {
        this.damage = damage;

        rb.linearVelocity = transform.up * velocity;
        ApplicationManager.instance.PlaySound("spikeshoot");

        Destroy(this.gameObject, 5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Events.PlayerTakeDamage(damage);
        }
        else if(collision.transform == this.transform)
        {
            return;
        }

        Destroy(this.gameObject);
    }
}
