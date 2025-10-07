using System;
using System.Collections;
using UnityEngine;
public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 5;

    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }

    int currentHealth;

    public int CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir, ProjectileConfig source)
    {
        if (IsDead) return;
        if (IsInvincible) return;

        currentHealth -= amount;


        DamageEventData damageEventData = new DamageEventData(this, amount, hitPoint, hitDir);
        GameEvents.Instance.OnDamage?.Invoke(damageEventData);

        if (currentHealth <= 0)
        {
            Die(damageEventData);
        }
    }

    public void Fall()
    {
        InstantDie(fallenToDeath: true);
    }

    public void InstantDie(bool fallenToDeath = false)
    {
        currentHealth = 0;
        IsDead = true;
        GameEvents.Instance.OnDeath?.Invoke(new DeathEventData(GetComponent<Entity>(), fallenToDeath: fallenToDeath));
    }

    void Die(DamageEventData damageEventData)
    {
        IsDead = true;
        GameEvents.Instance.OnDeath?.Invoke(new DeathEventData(GetComponent<Entity>(),damageEventData));
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }

    internal void SetInvincible(bool v)
    {
        IsInvincible = v;
    }
}