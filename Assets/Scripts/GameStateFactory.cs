using Zenject;

public class GameStateFactory : IGameStateFactory
{
    private DiContainer _container;

    public GameStateFactory(DiContainer container)
    {
        _container = container;
    }

    public IGameState CreateState(GameStateType type)
    {
        switch (type)
        {
            case GameStateType.Play:
                return _container.Instantiate<PlayState>();
            case GameStateType.Pause:
                return _container.Instantiate<PauseState>();
            case GameStateType.Lose:
                return _container.Instantiate<LoseState>();
            case GameStateType.Win:
                return _container.Instantiate<WinState>();
            // Add cases for other states
            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), $"Not expected state value: {type}");
        }
    }
}