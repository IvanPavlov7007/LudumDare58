using System;
using UnityEngine;

namespace Entities.Experimental
{
    [Obsolete]
    public class AliveStateBase : StateBase
    {
        public AliveStateBase(Entity entity) : base(entity)
        {
        }

        public override EntityState Type => EntityState.Alive;

        public override void OnEnter()
        {
            //gain controls
            //start AI
        }

        public override void OnDamage(DamageEventData damageEventData)
        {
            if (damageEventData.target != null && damageEventData.target == entity.Health as IDamageable)
            {
                stateController.setHitState(damageEventData);
            }
        }

        public override void OnDeath(DeathEventData deathEventData)
        {
            if(deathEventData.entity != null && deathEventData.entity == entity)
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