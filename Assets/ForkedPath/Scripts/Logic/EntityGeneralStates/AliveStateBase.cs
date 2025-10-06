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
            entity.StateMachine.ChangeState(new HitStateBase(entity, damageEventData));
        }
    }

    public override void OnDeath(DeathEventData deathEventData)
    {
        if(deathEventData.entity != null && deathEventData.entity == entity)
        {
            if (deathEventData.fallenToDeath)
            {
                entity.StateMachine.ChangeState(new FallingStateBase(entity, deathEventData));
            }
            else
            {
                entity.StateMachine.ChangeState(new DeadStateBase(entity, deathEventData));
            }
        }
    }
}