using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    ParticleSystem particle;
    TrailRenderer trail;

    Color bulletColor;
    float speed;
    float damage;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        particle = GetComponent<ParticleSystem>();
    }
    public void StartBullet(float _speed)
    {
        speed = _speed;

        // Pick a random color
        bulletColor = Random.ColorHSV();

        // Create gradient
        Gradient particleGradient = new Gradient();
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
}
