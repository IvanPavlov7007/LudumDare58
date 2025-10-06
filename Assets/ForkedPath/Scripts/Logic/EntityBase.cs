using Unity;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    public EntityStateMachine StateMachine { get; protected set; } = new EntityStateMachine();

    protected virtual void Update()
    {
        StateMachine.CurrentState?.Execute();
    }


}