using Entities.Experimental;
using System.Collections;
using UnityEngine;
namespace States.Experimental
{
    public class TomatoController : MonoBehaviour
    {
        Entity entity;
        EntityStateController stateController;
        private void Awake()
        {
            entity = GetComponent<Entity>();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => entity != null && entity.isInitialized);
            stateController = new TomatoStateController(entity);
            stateController.Initialize();
        }
    }

    public class TomatoStateController : EntityStateController
    {
        public TomatoStateController(Entity entity) : base(entity)
        {
        }

        protected override AliveStateBase getAliveState(Entity entity)
        {
            return null;// new MovingInOneDirectionAliveState(entity, entity.moveDirection);
        }

        protected override DeadStateBase getDeadState(Entity entity, DeathEventData deathEventData)
        {
            throw new System.NotImplementedException();
        }

        protected override FallingStateBase getFallingState(Entity entity, DeathEventData deathEventData)
        {
            throw new System.NotImplementedException();
        }

        protected override HitStateBase getHitState(Entity entity, DamageEventData damageEventData)
        {
            throw new System.NotImplementedException();
        }

        protected override InvincibleStateBase getInvincibleState(Entity entity, float duration = 0.5F)
        {
            throw new System.NotImplementedException();
        }
    }
}