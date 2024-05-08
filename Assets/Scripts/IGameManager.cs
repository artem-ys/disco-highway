public interface IGameManager
{
    void ChangeState(GameStateType newState);
    void EnablePlayerControl(bool isEnabled);
}