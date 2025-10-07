using System;
using System.Collections;
using UnityEngine;

namespace Entities.Experimental
{
    public class InvincibleStateBase : StateBase
    {
        protected float invincibilityDuration = 1.0f;
        public InvincibleStateBase(Entity entity, float invincibilityDuration) : base(entity)
        {
            this.invincibilityDuration = invincibilityDuration;
        }

        public override EntityState Type => EntityState.Invincible;

        override public void OnEnter()
        {
            base.OnEnter();
            entity.health.SetInvincible(true);
            entity.StartCoroutine(EndInvincibilityAfterDelay());
            // Play FX, animation, sound etc
        }

        protected virtual IEnumerator EndInvincibilityAfterDelay()
        {
            yield return new WaitForSeconds(invincibilityDuration);
            entity.health.SetInvincible(false);
            stateMachine.ChangeState(new AliveStateBase(entity));
        }

        public override void OnDamage(DamageEventData damageEventData)
        {
            if (damageEventData.target != null && damageEventData.target == entity.health)
            {
                Debug.LogError($"{entity.name} is invincible and should not take any damage");
            }
        }

        public override void OnDeath(DeathEventData deathEventData)
        {
            if (deathEventData.entity != null && deathEventData.entity == entity)
            {
                if (deathEventData.fallenToDeath)
                {
                    stateController.setFallingState(deathEventData);
                }
                else
                {
                    stateController.setDeadState(deathEventData);
                }
            }
        }
    }
}