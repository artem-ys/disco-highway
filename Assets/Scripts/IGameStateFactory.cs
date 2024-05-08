public interface IGameStateFactory
{
    IGameState CreateState(GameStateType type);
}