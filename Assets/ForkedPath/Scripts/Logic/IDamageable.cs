using System.Collections;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitNormal);
    bool IsDead { get; }
}