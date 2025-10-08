using UnityEngine;

public class DamageEventData
{
    public IDamageable target;
    public int damageAmount;
    public Vector2 hitPoint;
    public Vector2 hitDirection;

    public string context; // e.g. "Melee", "Projectile", "Explosion"

    public ScriptableObject config; // reference to any data asset (EnemyData, ProjectileData, etc.)
    public DamageEventData(IDamageable target, string context, int damageAmount, Vector2 hitPoint = default, Vector2 hitDirection = default, ScriptableObject config = null)
    {
        this.context = context;
        this.config = config;
        this.target = target;
        this.damageAmount = damageAmount;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
    }
}