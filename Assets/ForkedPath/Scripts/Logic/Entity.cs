using System;
using UnityEngine;

public partial class Entity : MonoBehaviour
{

    public GameObject deathFX;
    [SerializeField]
    private EntityConfig _config;
    public EntityConfig config { get { return _config; } }

    protected Health health;
    protected Rigidbody2D rb;
    protected bool isInitialized = false;

    public EntityStateMachine StateMachine { get; protected set; } = new EntityStateMachine();

    protected virtual void Update()
    {
        StateMachine.CurrentState?.Execute();
    }


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
    }

    protected virtual void OnDisable()
    {
        GameEvents.Instance.OnDeath -= HandleDeath;
    }

    protected virtual void HandleHit(IDamageable target, int amount, Vector2 hitPoint, Vector2 dir)
    {
        // Override in subclasses
    }

    protected virtual void HandleDeath(DeathEventData deathEventData)
    {
        
        //if ( target != null && target == health)
        //{
        //    if (deathFX) Instantiate(deathFX, transform.position, Quaternion.identity);
        //    GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Death", config));
        //    ScoreSystem.Instance.AddScore(scoreValue);
        //}
    }
}
