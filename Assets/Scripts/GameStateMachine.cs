using UniRx;
using Zenject;

public class GameStateMachine
{
    private IGameState _currentState;
    private IGameStateFactory _stateFactory;
 
    private ReactiveProperty<GameStateType> _currentGameStateType;
    public IReadOnlyReactiveProperty<GameStateType> CurrentGameStateType => _currentGameStateType;
    
    [Inject]
    public GameStateMachine(IGameStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
        _currentGameStateType = new ReactiveProperty<GameStateType>();
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

        _currentGameStateType.Value = newStateType;
    }

    public void Update()
    {
        _currentState?.UpdateState();
    }
}