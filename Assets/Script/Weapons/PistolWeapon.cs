using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStats))]

public class PistolWeapon : Weapon
{

    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform firePoint;

    private PlayerStats playerStats;
    public bool IsMagazineEmpty => currentAmmo <= 0;

    private int currentAmmo;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        currentAmmo = playerStats.MagazineSize;
    }

    public override bool TryShoot()
    {
        if (currentAmmo <= 0)
        {
            Reload();
            return false;
        }

        currentAmmo--;

        GameObject bulletObject = bulletPool.GetBullet();

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        bullet.Initialize(
            firePoint.right,
            playerStats.Attack,
            playerStats.ProjectileSpeed,
            playerStats.ProjectileLifetime
        );

        return true;
    }

    public override void Reload()
    {
        currentAmmo = playerStats.MagazineSize;

        Debug.Log("Reload");
    }

    public int CurrentAmmo => currentAmmo;

    public int MaxAmmo => playerStats.MagazineSize;
}