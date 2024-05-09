using UnityEditor;
using UnityEngine;

public class PlayState : IGameState
{
    private readonly IGameManager _gameManager;

    public PlayState(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void EnterState()
    {
        _gameManager.StartGame();
        
        Debug.Log("Game is now in Play State.");
    }

    public void UpdateState()
    {
        // If the pause button is pressed, change to PauseState
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameManager.ChangeState(GameStateType.Pause);
        }
    }

    public void ExitState()
    {
        
    }

    public void Dispose()
    {
    }
}