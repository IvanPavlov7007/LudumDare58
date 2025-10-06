using UnityEngine;
public class DeadStateBase : StateBase
{
    protected DeathEventData deathEventData;
    public DeadStateBase(Entity entity, DeathEventData deathEventData) : base(entity)
    {
        this.deathEventData = deathEventData;
    }

    public override EntityState Type => EntityState.Dead;

    public override void OnEnter()
    {
        // Disable controls, AI etc
    }

    public override void OnDamage(DamageEventData damageEventData)
    {
        if (damageEventData.target != null && damageEventData.target == entity.health)
        {
            Debug.LogError($"{entity.name} is dead and should not take any damage");
        }
    }

    public override void OnDeath(DeathEventData deathEventData)
    {
        if (deathEventData.entity != null)
        {
            Debug.LogError($"{entity.name} is already dead, can't be dead again");
        }
    }
}