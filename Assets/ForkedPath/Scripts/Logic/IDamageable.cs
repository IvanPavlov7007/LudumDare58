using System.Collections;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitNormal, ProjectileConfig source);
    public bool IsDead { get; }
}