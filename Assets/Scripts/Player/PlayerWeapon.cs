using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject bullet;

    [Header("Aiming Variables")]
    [SerializeField] Transform muzzle;
    [SerializeField] Transform playerTransform;
    //TODO weight variable that changes follow speed

    [Header("Shooting Variables")]
    [SerializeField] int shotsPerSecond;
    float shotCooldown;
    float lastShot;
    [SerializeField] int spreadAngle;
    [SerializeField] int bulletsPerShot;
    [SerializeField] float bulletVelocity;
    [SerializeField] float baseDamage;

    [Header("Charge Up")]
    [SerializeField] float minCharge; // Quick tap
    [SerializeField] float maxCharge; // Hold
    [SerializeField] float chargeSpeed;
    float currentCharge;
    [SerializeField] ParticleSystem chargeParticles;
    ParticleSystem.MainModule mainModule;
    Color nextColor;

    // Input
    InputAction playerAction;
    bool playerShooting;

    private void Start()
    {
        playerAction = InputSystem.actions.FindAction("Shoot");

        shotCooldown = 1f / shotsPerSecond;

        chargeParticles.Stop();
        mainModule = chargeParticles.main;
        mainModule.startColor = Color.yellow;
    }
    private void Update()
    {
        Look(); 

        lastShot += Time.deltaTime;
        if(lastShot > shotCooldown)
        {
            ChargeUp();
        }
    }
    void ChargeUp()
    {
        playerShooting = playerAction.IsPressed();

        if (playerAction.WasPerformedThisFrame())
        {
            Debug.Log("Pressed");

            nextColor = Color.HSVToRGB(Random.value, 1, 1);
            mainModule.startColor = nextColor;
            chargeParticles.Play();
            currentCharge = minCharge;
        }
        if (playerShooting)
        {
            Debug.Log("Charging");

            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minCharge, maxCharge);

            mainModule.startSize = currentCharge / 10f;

            Events.AddCameraZoom(currentCharge - minCharge);
        }
        if (playerAction.WasReleasedThisFrame())
        {
            Debug.Log("Shoot");

            chargeParticles.Stop();
            chargeParticles.Clear();

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Shoot(currentCharge);
            }
            currentCharge = 0f;
            lastShot = 0f;
        }
    }
    void Shoot(float size)
    {

        // Add spread
        Vector3 dir = playerTransform.up;
        float randSpread = Random.Range(-spreadAngle, spreadAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion bulletDir = Quaternion.Euler(0, 0, angle + randSpread);

        // Spawn Bullets
        GameObject _bulletGO = Instantiate(bullet, muzzle.position, bulletDir);
        Bullet _bullet = _bulletGO.GetComponent<Bullet>();
        _bullet.StartBullet(bulletVelocity, baseDamage, size, nextColor);
    }
    void Look()
    {
        // Rotate towards cursor
        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(screenMousePos);

        Vector2 lookDir = (Vector2)playerTransform.position - mousePos;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        playerTransform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }
}
