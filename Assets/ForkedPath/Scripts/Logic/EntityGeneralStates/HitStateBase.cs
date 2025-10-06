using UnityEngine;

/// <summary>
/// For animation, effects, control changes when hit but not dead
/// Might transition to Invincible or Dead state
/// </summary>
public class HitStateBase : StateBase
{
    protected DamageEventData damageEventData;
    protected float hitDuration = 0.5f; // Example duration
    protected float hitTimer = 0f;

    public HitStateBase(Entity entity, DamageEventData damageEventData,float hitDuration = 0.5f) : base(entity)
    {
        this.damageEventData = damageEventData;
        this.hitDuration = hitDuration;
        OnDamage(damageEventData);
    }

    public override EntityState Type => EntityState.Hit;

    public override void OnDamage(DamageEventData damageEventData)
    {
        hitTimer = 0f;
        // FX etc
    }

    public override void OnDeath(DeathEventData deathEventData)
    {
        if (deathEventData.entity != null && deathEventData.entity == entity)
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
