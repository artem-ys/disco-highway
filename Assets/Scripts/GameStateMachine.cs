using Zenject;

public class GameStateMachine
{
    private IGameState _currentState;
    private IGameStateFactory _stateFactory;

    [Inject]
    public GameStateMachine(IGameStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }
    
    public void ChangeState(GameStateType newStateType)
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
            _currentState.Dispose();
        }

        _currentState = _stateFactory.CreateState(newStateType);
        _currentState.EnterState();
    }

    public void Update()
    {
        _currentState?.UpdateState();
    }
}