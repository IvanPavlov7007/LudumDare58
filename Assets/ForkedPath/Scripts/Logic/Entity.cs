using System;
using System.Collections;
using UnityEngine;

public partial class Entity : MonoBehaviour
{

    public GameObject deathFX;
    [SerializeField]
    private EntityConfig _config;
    public EntityConfig config { get { return _config; } }

    public Health health { get; protected set; }
    public Rigidbody2D rb { get; protected set; }
    public bool isInitialized { get; protected set; } = false;

    protected EntityState state = EntityState.Alive;
    public EntityState CurrentState => state;

    public event Action<EntityState> StateChanged;

    protected Coroutine invinsibilityRoutine;

    //shared data for states to use
    public Vector2 moveDirection;


    public virtual void Initialize(EntityConfig config)
    {
        if (isInitialized) return;
        _config = config;
        if (health == null) health = gameObject.AddComponent<Health>();
        if (health != null && config != null)
        {
            health.SetMaxHealth(config.maxHealth);
            health.ResetHealth();
        }
        isInitialized = true;
    }

    public virtual void SetMoveDirection(Vector2 normalized)
    {
        rb.linearVelocity = normalized * config.moveSpeed;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if(!isInitialized && config != null)
            Initialize(config);
    }

    protected virtual void OnEnable()
    {
        GameEvents.Instance.OnDeath += HandleDeath;
        GameEvents.Instance.OnDamage += HandleHit;
    }

    protected virtual void OnDisable()
    {
        GameEvents.Instance.OnDeath -= HandleDeath;
        GameEvents.Instance.OnDamage -= HandleHit;
    }

    protected virtual void ChangeState(EntityState newState)
    {
        ExitState(state);
        if (state == newState) Debug.LogWarning($"{name} is reentering to same state {newState}");
        state = newState;
        EnterState(newState);
        StateChanged?.Invoke(state);
    }

    protected virtual void ExitState(EntityState oldState)
    {
        switch (oldState)
        {
            case EntityState.Invincible:
                if (invinsibilityRoutine != null)
                {
                    StopCoroutine(invinsibilityRoutine);
                    invinsibilityRoutine = null;
                }
                health.SetInvincible(false);
                break;
        }
    }

    protected virtual void EnterState(EntityState newState)
    {
        switch (newState)
        {
            case EntityState.Invincible:
                if (invinsibilityRoutine != null) StopCoroutine(invinsibilityRoutine);
                invinsibilityRoutine = StartCoroutine(InvincibleForSeconds(config.invincibilityDuration));
                break;
        }
    }

    private IEnumerator InvincibleForSeconds(float duration)
    {
        health.SetInvincible(true);
        yield return new WaitForSeconds(duration);
        health.SetInvincible(false);
    }

    protected virtual void HandleHit(DamageEventData damageEventData)
    {
        if (health == null || damageEventData.target != health) return;

        switch (CurrentState)
        {
            case EntityState.Alive:
            case EntityState.Hit:
                ChangeState(EntityState.Hit);
                if(config.invincibleAfterHit && health.CurrentHealth > 0)
                    ChangeState(EntityState.Invincible);
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

        if(deathEventData.fallenToDeath)
        {
            switch(CurrentState)
            {
                case EntityState.Alive:
                case EntityState.Hit:
                case EntityState.Invincible:
                    ChangeState(EntityState.Falling);
                    break;
                case EntityState.Dead:
                    ChangeState(EntityState.DeadFalling);
                    break;
                case EntityState.DeadFalling:
                    Debug.LogError($"{name} is already dead falling");
                    break;
                case EntityState.Falling:
                    Debug.LogError($"{name} is already falling and should not fall to death again");
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

        //if ( target != null && target == health)
        //{
        //    if (deathFX) Instantiate(deathFX, transform.position, Quaternion.identity);
        //    GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Death", config));
        //    ScoreSystem.Instance.AddScore(scoreValue);
        //}
    }
}