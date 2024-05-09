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
        _gameManager.PrepareGame();
        
        Debug.Log("Game is now Paused.");
    }

    public void UpdateState()
    {
        // If the pause button is pressed again, resume the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameManager.ChangeState(GameStateType.Play);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _gameManager.ChangeState(GameStateType.Play); // Or GameStateType.NextLevel if applicable
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