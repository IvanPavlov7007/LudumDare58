using System;
using UnityEngine;

namespace Entities.Experimental
{
    [Obsolete]
    public class FallingStateBase : StateBase
    {
        protected DeathEventData deathEventData;
        public FallingStateBase(Entity entity, DeathEventData deathEventData) : base(entity)
        {
            this.deathEventData = deathEventData;
        }

        public override EntityState Type => EntityState.Falling;

        public override void OnEnter()
        {
            // Disable controls, AI etc
        }

        public override void OnDamage(DamageEventData damageEventData)
        {
            if (damageEventData.target != null && damageEventData.target == entity.Health as IDamageable)
            {
                Debug.LogError($"{entity.name} is dead and should not take any damage");
            }
        }

        public override void OnDeath(DeathEventData deathEventData)
        {
            if (deathEventData.entity != null)
            {
                Debug.LogError($"{entity.name} is already falling, can't be dead again");
            }
        }
    }
}