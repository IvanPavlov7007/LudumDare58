using UnityEngine;
public class EntityStateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState?.OnEnter();
    }

    public bool IsInState(EntityState type)
    {
        return CurrentState != null && CurrentState.Type == type;
    }
}

public enum EntityState
{
    Alive,
    Dead,
    Hit,
    Falling,
    Invincible,
}

public interface IState
{
    EntityState Type { get; }
    void OnEnter();
    void OnUpdate();
    void OnFixedUpdate();
    void OnExit();
    void OnDamage(DamageEventData damageEventData);
    void OnDeath(DeathEventData deathEventData);
}

public abstract class StateBase : IState
{
    protected Entity entity;

    public StateBase(Entity entity)
    {
        this.entity = entity;
    }

    public abstract EntityState Type { get; }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnExit() { }
    public abstract void OnDamage(DamageEventData damageEventData);
    public abstract void OnDeath(DeathEventData deathEventData);
}