using UnityEngine;
public class EntityStateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public bool IsInState(EntityState type)
    {
        return CurrentState != null && CurrentState.Type == type;
    }
}

public enum EntityState
{
    Alive,
    Dead,
    Falling,
    Invincible,
}

public interface IState
{
    EntityState Type { get; }
    void Enter();
    void Execute();
    void Exit();
}