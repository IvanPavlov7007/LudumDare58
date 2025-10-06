using System;
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

    public EntityStateMachine StateMachine { get; protected set; } = new EntityStateMachine();
    public EntityStateController StateController { get; set; }

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
        Initialize(config);
        rb = GetComponent<Rigidbody2D>();
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

    protected virtual void Update()
    {
        StateMachine.CurrentState?.OnUpdate();
    }

    protected virtual void FixedUpdate()
    {
        StateMachine.CurrentState?.OnFixedUpdate();
    }

    protected virtual void HandleHit(DamageEventData damageEventData)
    {
        StateMachine.CurrentState?.OnDamage(damageEventData);
    }

    protected virtual void HandleDeath(DeathEventData deathEventData)
    {
        StateMachine.CurrentState?.OnDeath(deathEventData);
        //if ( target != null && target == health)
        //{
        //    if (deathFX) Instantiate(deathFX, transform.position, Quaternion.identity);
        //    GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Death", config));
        //    ScoreSystem.Instance.AddScore(scoreValue);
        //}
    }

    public virtual void Fall()
    {

    }
}
