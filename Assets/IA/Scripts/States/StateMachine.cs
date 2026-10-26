using System;
using System.Collections.Generic;

public class StateMachine
{
    public State CurrentState { get; private set; }
    readonly Dictionary<Enum, State> states = new();

    public void RegisterState(Enum key, State state)
    {
        states[key] = state;
    }

    public void ChangeState(Enum key)
    {
        State newState = states[key];

        if (newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}