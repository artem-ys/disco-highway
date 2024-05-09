public interface IGameManager
{
    void ChangeState(GameStateType newState);
    void EnablePlayerControl(bool isEnabled);
    void StopGame(bool b);
    void StartGame();
    void PrepareGame();
}