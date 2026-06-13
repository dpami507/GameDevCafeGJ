using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using static UnityEngine.ParticleSystem;
using static UpgradeManager;

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

        CalculateShotCooldown();

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

        CalculateShotCooldown();
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
            if(chargeParticles.isPlaying == false)
                chargeParticles.Play();

            currentCharge += GetChargeSpeed() * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minCharge, GetMaxShotSize());

            mainModule.startSize = currentCharge / 10f;

            Events.AddCameraZoom(currentCharge - minCharge);
        }
        if (playerAction.WasReleasedThisFrame())
        {
            Debug.Log("Shoot");
            Events.PlaySound("shoot");

            chargeParticles.Stop();
            chargeParticles.Clear();

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Shoot(CalculateShotSize());
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
        _bullet.StartBullet(bulletVelocity, CalculateBaseDamage(), size, nextColor);
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
    void CalculateShotCooldown() => shotCooldown = 1f / (shotsPerSecond * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.AttackSpeed));
    float CalculateBaseDamage() => baseDamage * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.AttackDamange);
    float CalculateShotSize() => currentCharge * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.AttackSize);
    float GetChargeSpeed() => chargeSpeed * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.ChargeSpeed);
    float GetMaxShotSize() => maxCharge * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.MaxCharge);
}
