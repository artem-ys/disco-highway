using UnityEngine;

public class PauseState : IGameState
{
    private readonly IGameManager _gameManager;

    public PauseState(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void EnterState()
    {
        // Pause the game, perhaps enable a pause menu
        Time.timeScale = 0;
        Debug.Log("Game is now Paused.");
    }

    public void UpdateState()
    {
        // If the pause button is pressed again, resume the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameManager.ChangeState(GameStateType.Play);
        }
    }

    public void ExitState()
    {
        // Resume game time
        Time.timeScale = 1;
    }

    public void Dispose()
    {
    }
}