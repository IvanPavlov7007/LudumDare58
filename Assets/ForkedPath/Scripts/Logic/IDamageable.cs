using System.Collections;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, string context, Vector2 hitPoint, Vector2 hitNormal, ScriptableObject sourceConfig);
    public bool IsDead { get; }
}