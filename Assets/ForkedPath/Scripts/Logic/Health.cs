using System;
using System.Collections;
using UnityEngine;
public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 5;
    public bool destroyOnDeath = true;

    [Header("Invulnerability")]
    public bool useIFrames = false;
    public float iFrameDuration = 0.2f;
    float iFrameTimer;

    public bool IsDead { get; private set; }

    int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir, ProjectileConfig source)
    {
        if (IsDead) return;
        if (useIFrames && iFrameTimer > 0f) return;

        currentHealth -= amount;
        iFrameTimer = iFrameDuration;

        GameEvents.Instance.OnDamage?.Invoke(this, amount, hitPoint, hitDir);

        if (currentHealth <= 0)
        {
            Die(hitPoint, hitDir);
        }
    }

    void Update()
    {
        if (iFrameTimer > 0f)
            iFrameTimer -= Time.deltaTime;
    }

    void Die(Vector2 hitPoint, Vector2 hitDir)
    {
        IsDead = true;
        GameEvents.Instance.OnDeath?.Invoke(this, hitPoint, hitDir);

        if (destroyOnDeath)
            Destroy(gameObject);
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
}