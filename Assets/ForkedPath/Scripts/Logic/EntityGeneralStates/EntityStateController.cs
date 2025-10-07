using System.Collections;
using UnityEngine;

namespace Entities.Experimental
{
    public abstract class EntityStateController {
        protected Entity entity;
        protected EntityStateMachine stateMachine;

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
            if (stateMachine.IsInState(EntityState.Alive)) return;
            var aliveState = getAliveState(entity);
            stateMachine.ChangeState(aliveState);
        }

        public virtual void setDeadState(DeathEventData deathEventData)
        {
            if (stateMachine.IsInState(EntityState.Dead)) return;
            var deadState = getDeadState(entity, deathEventData);
            stateMachine.ChangeState(deadState);
        }

        public virtual void setFallingState(DeathEventData deathEventData)
        {
            if (stateMachine.IsInState(EntityState.Falling)) return;
            var fallingState = getFallingState(entity, deathEventData);
            stateMachine.ChangeState(fallingState);
        }

        public virtual void setHitState(DamageEventData damageEventData)
        {
            if (stateMachine.IsInState(EntityState.Hit)) return;
            var hitState = getHitState(entity, damageEventData);
            stateMachine.ChangeState(hitState);
        }

        public virtual void setInvincibleState(float duration = 0.5f)
        {
            if (stateMachine.IsInState(EntityState.Invincible)) return;
            var invincibleState = getInvincibleState(entity, duration);
            stateMachine.ChangeState(invincibleState);
        }

        protected abstract AliveStateBase getAliveState(Entity entity);

        protected abstract DeadStateBase getDeadState(Entity entity, DeathEventData deathEventData);

        protected abstract FallingStateBase getFallingState(Entity entity, DeathEventData deathEventData);

        protected abstract HitStateBase getHitState(Entity entity, DamageEventData damageEventData);

        protected abstract InvincibleStateBase getInvincibleState(Entity entity, float duration = 0.5f);

    }
}