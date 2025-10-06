using UnityEngine;

public class DamageEventData
{
    public IDamageable target;
    public int damageAmount;
    public Vector2 hitPoint;
    public Vector2 hitDirection;
    public DamageEventData(IDamageable target, int damageAmount, Vector2 hitPoint = default, Vector2 hitDirection = default)
    {
        this.target = target;
        this.damageAmount = damageAmount;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
    }
}