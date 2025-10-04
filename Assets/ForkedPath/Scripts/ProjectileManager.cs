using System.Collections;
using UnityEngine;
using Pixelplacement;

public class ProjectileManager : Singleton<ProjectileManager>
{
    public void Shoot(Vector2 shootOrigin, Vector2 direction, ProjectileConfig config)
    {
        if (config == null || config.projectilePrefab == null) return;
        GameObject projectileInstance = Instantiate(config.projectilePrefab, shootOrigin, Quaternion.identity);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction.normalized * config.projectileSpeed, config.layerMask, config.damage, config.maxLifetime);
        }
    }
}