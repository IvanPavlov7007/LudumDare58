using System.Collections;
using UnityEngine;

public abstract class EntityStateController {
    protected Entity entity;

    public EntityStateController(Entity entity)
    {
        this.entity = entity;
    }

    public void Initialize()
    {
        setAliveState();
    }

    public virtual void setAliveState()
    {
                if (entity.StateMachine.IsInState(EntityState.Alive)) return;
        var aliveState = getAliveState(entity);
        entity.StateMachine.ChangeState(aliveState);
    }

    public virtual void setDeadState(DeathEventData deathEventData)
    {
                if (entity.StateMachine.IsInState(EntityState.Dead)) return;
        var deadState = getDeadState(entity, deathEventData);
        entity.StateMachine.ChangeState(deadState);
    }

    public virtual void setFallingState(DeathEventData deathEventData)
    {
                if (entity.StateMachine.IsInState(EntityState.Falling)) return;
        var fallingState = getFallingState(entity, deathEventData);
        entity.StateMachine.ChangeState(fallingState);
    }

    public virtual void setHitState(DamageEventData damageEventData)
    {
                if (entity.StateMachine.IsInState(EntityState.Hit)) return;
        var hitState = getHitState(entity, damageEventData);
        entity.StateMachine.ChangeState(hitState);
    }

    public virtual void setInvincibleState(float duration = 0.5f)
    {
                if (entity.StateMachine.IsInState(EntityState.Invincible)) return;
        var invincibleState = getInvincibleState(entity, duration);
        entity.StateMachine.ChangeState(invincibleState);
    }

    protected abstract AliveStateBase getAliveState(Entity entity);

    protected abstract DeadStateBase getDeadState(Entity entity, DeathEventData deathEventData);

    protected abstract FallingStateBase getFallingState(Entity entity, DeathEventData deathEventData);

    protected abstract HitStateBase getHitState(Entity entity, DamageEventData damageEventData);

    protected abstract InvincibleStateBase getInvincibleState(Entity entity, float duration = 0.5f);

}