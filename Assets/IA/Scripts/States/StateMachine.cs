public class StateMachine
{
    public State CurrentState { get; private set; }

    public void ChangeState(State newState)
    {
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