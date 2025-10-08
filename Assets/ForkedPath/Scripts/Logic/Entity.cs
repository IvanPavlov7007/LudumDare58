using System;
using System.Collections;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    [SerializeField]
    private EntityConfig _config;
    public EntityConfig Config { get { return _config; } }

    public Health Health { get; protected set; }
    public Rigidbody2D Rb { get; protected set; }
    public bool IsInitialized { get; protected set; } = false;

    protected EntityState state = EntityState.Alive;
    public EntityState CurrentState => state;

    public event Action<EntityState> StateChanged;

    protected Coroutine invincibilityCoroutine;
    protected Coroutine hitStunCoroutine; // NEW

    public virtual void Initialize(EntityConfig config)
    {
        if (IsInitialized) return;
        _config = config;
        if (Health == null) Health = gameObject.AddComponent<Health>();
        if (Health != null && config != null)
        {
            Health.SetMaxHealth(config.maxHealth);
            Health.ResetHealth();
        }
        IsInitialized = true;
    }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if(!IsInitialized && Config != null)
            Initialize(Config);
    }

    protected virtual void OnEnable()
    {
        GameEvents.Instance.OnDeath += HandleDeath;
        GameEvents.Instance.OnDamage += HandleHit;
        GameEvents.Instance.OnInvincibilityChanged += OnInvincibilityChanged;
        GameEvents.Instance.OnFallingToDeathStarted += OnFallingToDeathStarted;
        GameEvents.Instance.OnCorpseLanded += OnCorpseLanded;
    }

    protected virtual void OnDisable()
    {
        GameEvents.Instance.OnDeath -= HandleDeath;
        GameEvents.Instance.OnDamage -= HandleHit;
        GameEvents.Instance.OnInvincibilityChanged -= OnInvincibilityChanged;
        GameEvents.Instance.OnFallingToDeathStarted -= OnFallingToDeathStarted;
        GameEvents.Instance.OnCorpseLanded -= OnCorpseLanded;

        CancelHitStun(); // NEW
    }

    protected virtual void ChangeState(EntityState newState)
    {
        ExitState(state);
        if (state == newState) Debug.LogWarning($"{name} is reentering to same state {newState}");
        state = newState;
        EnterState(newState);
        StateChanged?.Invoke(state);
    }

    protected virtual void ExitState(EntityState oldState) { }
    protected virtual void EnterState(EntityState newState) { }

    protected virtual void HandleHit(DamageEventData damageEventData)
    {
        if (Health == null || damageEventData.target != Health as IDamageable) return;

        switch (CurrentState)
        {
            case EntityState.Alive:
            case EntityState.Hit:
                ChangeState(EntityState.Hit);

                bool willGoInvincible = Config.invincibleAfterHit && Health.CurrentHealth > 0;
                if (willGoInvincible)
                {
                    // invincibility owns returning to Alive via event
                    CancelHitStun();
                    Health.BeginInvincibility(Config.invincibilityDuration);
                }
                else
                {
                    float stun = Mathf.Max(0f, Config.hitStunDuration);
                    if (stun <= 0f)
                    {
                        ChangeState(EntityState.Alive);
                    }
                    else
                    {
                        RestartHitStun(stun);
                    }
                }
                break;

            case EntityState.Dead:
                Debug.LogError($"{name} is dead and should not take any damage");
                break;
            case EntityState.Falling:
                Debug.LogError($"{name} is falling and should not take any damage");
                break;
            case EntityState.Invincible:
                Debug.LogError($"{name} is invincible and should not take any damage");
                break;
        }
    }

    protected virtual void HandleDeath(DeathEventData deathEventData)
    {
        if(deathEventData.entity != this) return;

        // any death path should cancel pending hit-stun
        CancelHitStun();

        if(deathEventData.fallenToDeath)
        {
            switch(CurrentState)
            {
                case EntityState.Alive:
                case EntityState.Hit:
                case EntityState.Invincible:
                    ChangeState(EntityState.Falling);
                    break;
                case EntityState.Falling:
                    // fall finalized: transition to dead now
                    ChangeState(EntityState.Dead);
                    break;
                case EntityState.Dead:
                    ChangeState(EntityState.DeadFalling);
                    break;
                case EntityState.DeadFalling:
                    Debug.LogError($"{name} is already dead falling");
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case EntityState.Alive:
                case EntityState.Hit:
                case EntityState.Invincible:
                    ChangeState(EntityState.Dead);
                    break;
                case EntityState.Dead:
                    Debug.LogError($"{name} is already dead");
                    break;
                case EntityState.Falling:
                    Debug.LogError($"{name} is falling and should not die again");
                    break;
            }
        }
    }

    void OnInvincibilityChanged(InvincibilityEventData e)
    {
        if (e.Entity != this) return;
        if (e.IsInvincible)
        {
            CancelHitStun(); // avoid racing the stun timer
            ChangeState(EntityState.Invincible);
        }
        else if (CurrentState == EntityState.Invincible && !Health.IsDead)
        {
            ChangeState(EntityState.Alive);
        }
    }

    void OnFallingToDeathStarted(FallingEventData e)
    {
        if (e.entity != this) return;

        CancelHitStun(); // cancel stun if we start falling

        switch (CurrentState)
        {
            case EntityState.Alive:
            case EntityState.Hit:
            case EntityState.Invincible:
                ChangeState(EntityState.Falling);
                break;
            case EntityState.Dead:
                ChangeState(EntityState.DeadFalling);
                break;
            default:
                // if already Falling/DeadFalling, ignore
                Debug.LogWarning($"{name} should not start falling to death from state {CurrentState}");
                break;
        }
    }

    void OnCorpseLanded(CorpseLandedEventData e)
    {
        if (e.entity != this) return;
        if (CurrentState == EntityState.DeadFalling)
        {
            ChangeState(EntityState.Dead);
        }
    }

    // --- Hit-stun helpers ---
    void RestartHitStun(float duration)
    {
        CancelHitStun();
        hitStunCoroutine = StartCoroutine(HitStunDelay(duration));
    }

    void CancelHitStun()
    {
        if (hitStunCoroutine != null)
        {
            StopCoroutine(hitStunCoroutine);
            hitStunCoroutine = null;
        }
    }

    IEnumerator HitStunDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        hitStunCoroutine = null;

        // Only return to Alive if still stunned and not dead/falling
        if (CurrentState == EntityState.Hit && !Health.IsDead && !Health.IsFallingToDeath)
        {
            ChangeState(EntityState.Alive);
        }
    }
}