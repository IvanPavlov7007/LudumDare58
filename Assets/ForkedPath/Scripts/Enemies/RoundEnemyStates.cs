using System.Collections;
using UnityEngine;

public class MovingInOneDirectionAliveState : AliveStateBase
{
    Vector2 moveDir;
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public MovingInOneDirectionAliveState(Entity entity, Vector2 moveDir) : base(entity)
    {
        this.moveDir = moveDir;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        entity.rb.linearVelocity = moveDir * entity.config.moveSpeed;
    }
}