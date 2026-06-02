using UnityEngine;
using UnityEngine.InputSystem;

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

    // Input
    InputAction playerAction;
    bool playerShooting;

    private void Start()
    {
        playerAction = InputSystem.actions.FindAction("Shoot");

        shotCooldown = 1f / shotsPerSecond;
    }
    private void Update()
    {
        Look(); 

        playerShooting = playerAction.IsPressed();

        lastShot += Time.deltaTime;
        if(playerShooting && lastShot > shotCooldown)
        {
            for (int i = 0; i < bulletsPerShot; i++)
            {
                Shoot();
            }
            lastShot = 0f;
        }
    }
    void Shoot()
    {
        // Add spread
        Vector3 dir = playerTransform.up;
        float randSpread = Random.Range(-spreadAngle, spreadAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion bulletDir = Quaternion.Euler(0, 0, angle + randSpread);

        // Spawn Bullets
        GameObject _bullet = Instantiate(bullet, muzzle.position, bulletDir);
        Rigidbody2D _bulletRB = _bullet.GetComponent<Rigidbody2D>();
        _bulletRB.linearVelocity = _bullet.transform.right * bulletVelocity;

        // Kill after 5 seconds
        Destroy(_bullet, 5f);
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
