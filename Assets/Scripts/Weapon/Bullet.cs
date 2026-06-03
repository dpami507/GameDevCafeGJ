using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject explosionParticle;

    Rigidbody2D rb;
    ParticleSystem particle;
    TrailRenderer trail;

    Gradient particleGradient;
    float size;

    Color bulletColor;
    float speed;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        particle = GetComponent<ParticleSystem>();
    }
    public void StartBullet(float _speed, float _size, Color _color)
    {
        speed = _speed;
        size = _size;

        // Set scale
        transform.localScale = Vector3.one * size;

        // Pick a random color
        bulletColor = _color;

        // Create gradient
        particleGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[1];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        colorKeys[0].color = bulletColor;
        colorKeys[0].time = 0f;

        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;

        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;

        particleGradient.SetKeys(colorKeys, alphaKeys);

        // Use the color
        trail.colorGradient = particleGradient;

        // Set the particle system
        ParticleSystem.ColorOverLifetimeModule col = particle.colorOverLifetime;
        ParticleSystem.MinMaxGradient gr = new ParticleSystem.MinMaxGradient(particleGradient);
        gr.mode = ParticleSystemGradientMode.Gradient;
        col.color = gr;

        // Set Speed
        rb.linearVelocity = transform.right * speed;

        // Kill after 5 seconds
        Destroy(this.gameObject, 5f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            return;
        }
        Explode(collision);

        // Change tile color
        DungeonGeneration gen = FindFirstObjectByType<DungeonGeneration>();
        if (gen == null)
            return;

        Vector2 contactPoint = collision.contacts[0].point + (Vector2)(collision.contacts[0].normal * -0.1f);
        gen.SetTilesToColor(contactPoint, size, bulletColor);
    }
    void Explode(Collision2D collision)
    {
        // Create GameObject
        GameObject explosionParticleGO = Instantiate(explosionParticle, transform.position, Quaternion.identity);

        // Set the particle system
        ParticleSystem.ColorOverLifetimeModule col = explosionParticleGO.GetComponent<ParticleSystem>().colorOverLifetime;
        ParticleSystem.MinMaxGradient gr = new ParticleSystem.MinMaxGradient(particleGradient);
        gr.mode = ParticleSystemGradientMode.Gradient;
        col.color = gr;

        // Cleanup time
        Destroy(explosionParticleGO, 1f);
        Destroy(this.gameObject);
    }
}
