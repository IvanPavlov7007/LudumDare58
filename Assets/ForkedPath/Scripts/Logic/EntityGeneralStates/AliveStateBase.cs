using UnityEngine;
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
        if (damageEventData.target != null && damageEventData.target == entity.health)
        {
            entity.StateController.setHitState(damageEventData);
        }
    }

    public override void OnDeath(DeathEventData deathEventData)
    {
        if(deathEventData.entity != null && deathEventData.entity == entity)
        {
            if (deathEventData.fallenToDeath)
            {
                entity.StateController.setFallingState(deathEventData);
            }
            else
            {
                entity.StateController.setDeadState(deathEventData);
            }
        }
    }
}